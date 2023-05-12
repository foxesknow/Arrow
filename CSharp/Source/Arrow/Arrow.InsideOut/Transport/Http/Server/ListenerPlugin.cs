using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using Arrow.Execution;
using Arrow.Logging;
using Arrow.InsideOut.Transport.Implementation;
using System.Threading;
using Arrow.Threading.Tasks;
using System.Net.Mime;
using Arrow.IO;
using System.IO.Compression;

namespace Arrow.InsideOut.Transport.Http.Server;

/// <summary>
/// Listens on HTTP for calls to Process/GetDetails and Process/Execute over POST.
/// The listener can optionally be configures to listen to /GetDetails over GET to
/// allow browsers to query for information easily
/// </summary>
public sealed class ListenerPlugin : ListenerPluginBase
{
    private const string ListenerName = "InsideOut.Http.ListenerPlugin";

    private static readonly ILog Log = new PrefixLog(LogManager.GetDefaultLog(), $"[{ListenerName}]");

    private readonly HashSet<string> m_ProcessPaths = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> m_GetDetailsPaths = new(StringComparer.OrdinalIgnoreCase);

    private readonly CancellationTokenSource m_Cts = new();

    private readonly InsideOutJsonSerializer m_Serializer = new();

    private HttpListener? m_Listener;

    private Task? m_ListenTask;

    public override string Name => ListenerName;

    protected override ValueTask Start()
    {
        if(HttpListener.IsSupported == false) throw new InvalidOperationException("http not supported");
        if(string.IsNullOrWhiteSpace(this.InstanceName)) throw new InsideOutException("no instance name set");
        if(this.MaxConnections < 1) throw new InsideOutException("need at least one connection");

        var publisherID = new PublisherID(Environment.MachineName, this.InstanceName);
        Register(publisherID);

        m_Listener = new HttpListener();

        foreach(var prefix in this.Prefixes)
        {
            m_Listener.Prefixes.Add(prefix);

            var (getDetailsPath, processPath) = ExtractPaths(prefix);
            m_GetDetailsPaths.Add(getDetailsPath);
            m_ProcessPaths.Add(processPath);
        }

        //If this fails then it's probably because you don't have sufficient rights to start the listener.
        // You can get around this by running as admin (gulp!)
        m_Listener.Start();

        m_ListenTask = Task.Run(() => Listen(m_Listener, this.MaxConnections, m_Cts.Token));
        Log.Info("started");

        return default;
    }

    protected override async ValueTask Stop()
    {
        Log.Info("stopping");

        m_Cts.Cancel();
        m_Listener?.Stop();

        if(m_ListenTask is not null)
        {
            await m_ListenTask.ContinueOnAnyContext();
            m_ListenTask = null;
        }

        m_Cts.Dispose();
        m_Listener?.Close();
        m_Listener = null;
    }

    /// <summary>
    /// The http prefixes to listen on
    /// </summary>
    public List<string> Prefixes{get;} = new();

    /// <summary>
    /// How many connections to allow at any one time
    /// </summary>
    public int MaxConnections{get; set;} = 8;

    /// <summary>
    /// Logs the incoming json when process is called
    /// </summary>
    public bool LogIncomingJson{get; set;} = false;

    /// <summary>
    /// Allows client to get InsideOut details.
    /// </summary>
    public bool EnableGetDetails{get; set;} = false;

    private async Task Listen(HttpListener listener, int maxConnections, CancellationToken ct)
    {
        using(var throttle = new SemaphoreSlim(maxConnections, maxConnections))
        {
            try
            {
                while(ct.IsCancellationRequested == false)
                {
                    await throttle.WaitAsync(ct).ContinueOnAnyContext();
                    var context = await listener.GetContextAsync().ContinueOnAnyContext();

                    // Handle the request, but there's no need to await it
                    var _ = Task.Run(async () =>
                    {
                        await HandleRequest(context, ct).ContinueOnAnyContext();
                        throttle.Release();
                    });

                }
            }
            catch(Exception e)
            {
                if(ct.IsCancellationRequested == false)
                {
                    Log.Error("stopped unexpectedly", e);
                }
            }
        }
    }

    private async ValueTask HandleRequest(HttpListenerContext context, CancellationToken ct)
    {
        using(context.Response)
        {
            try
            {
                if(context.Request.Url is null) throw new InsideOutException("no url");

                var path = context.Request.Url.LocalPath;
                if(m_ProcessPaths.Contains(path))
                {
                    await HandleProcess(context, ct).ContinueOnAnyContext();
                }
                else if(this.EnableGetDetails && m_GetDetailsPaths.Contains(path))
                {
                    await HandleGetDetails(context, ct).ContinueOnAnyContext();
                }
                else
                {
                    await SendError(context, "invalid url", HttpStatusCode.BadRequest).ContinueOnAnyContext();
                }
            }
            catch(Exception e)
            {
                Log.Error("failed to handle request", e);
                await MethodCall.AllowFailAsync(async () => await SendError(context, e.Message, HttpStatusCode.InternalServerError)).ContinueOnAnyContext();
            }
        }
    }

    private async ValueTask HandleGetDetails(HttpListenerContext context, CancellationToken ct)
    {
        var request = context.Request;
        RequireHttpMethod(request, "GET");

        var node = GetRoot();
        if(node is null) throw new InsideOutException("no root node");

        var details = await node.GetDetails(ct).ContinueOnAnyContext();
        var json = m_Serializer.Serialize(details);
        await SendReponse(context, MimeType.Json, json, HttpStatusCode.OK).ContinueOnAnyContext();
    }

    private async ValueTask HandleProcess(HttpListenerContext context, CancellationToken ct)
    {
        var request = context.Request;
        RequireHttpMethod(request, "POST");
        RequireContentType(request, MimeType.Json);

        using(var stream = request.InputStream)
        {
            var transportRequest = m_Serializer.Deserialize<TransportRequest>(stream);
            if(transportRequest is null) throw new InsideOutException("no transport request sent");

            if(this.LogIncomingJson)
            {
                var json = m_Serializer.Serialize(transportRequest);
                Log.Info(json);
            }

            var (success, transportResponse) = await Process(transportRequest, ct).ContinueOnAnyContext();
            if(success)
            {
                if(transportResponse is null)
                {
                    await SendError(context, "failed to generate a response", HttpStatusCode.InternalServerError).ContinueOnAnyContext();
                }
                else
                {
                    var responseJson = m_Serializer.Serialize(transportResponse);
                    await SendJson(context, responseJson, HttpStatusCode.OK).ContinueOnAnyContext();
                }
            }
            else
            {
                await SendError(context, "failed to process request", HttpStatusCode.BadRequest).ContinueOnAnyContext();
            }
        }
    }

    private ValueTask SendJson(HttpListenerContext context, string json, HttpStatusCode statusCode)
    {
        return SendReponse(context, MimeType.Json, json, statusCode);
    }

    private ValueTask SendError(HttpListenerContext context, string text, HttpStatusCode statusCode)
    {
        return SendReponse(context, MimeType.Text, text, statusCode);
    }

    private async ValueTask SendReponse(HttpListenerContext context, string contentType, string text, HttpStatusCode statusCode)
    {
        var response = context.Response;
        response.ContentType = contentType;
        response.StatusCode = (int)statusCode;
        response.AddHeader("Content-Encoding", "gzip");

        using(var buffer = Compress(text))
        using(var stream = response.OutputStream)
        {
            response.ContentLength64 = buffer.Length;
            await stream.WriteAsync(buffer.Buffer!, 0, buffer.Length).ContinueOnAnyContext();
        }
    }

    private ArrayPoolReturner<byte> Compress(string text)
    {
        using(var stream = new ArrayPoolMemoryStream(text.Length))
        {
            using(var zip = new GZipStream(stream, CompressionMode.Compress, true))
            {
                var buffer = Encoding.UTF8.GetBytes(text);
                zip.Write(buffer, 0, buffer.Length);
            }

            return stream.Detach();
        }
    }

    private (string GetDetailsPath, string ProcessPath) ExtractPaths(string prefix)
    {
        // Prefixes always end with a /, so we'll be ok
        var pivot = prefix.IndexOf("//");
        var pathStart = prefix.IndexOf('/', pivot + 2);

        var getDetailsPath = prefix.Substring(pathStart) + "GetDetails";
        var processPath = prefix.Substring(pathStart) + "Process";

        return (getDetailsPath, processPath);
    }

    private void RequireHttpMethod(HttpListenerRequest request, string method)
    {
        if(string.Equals(method, request.HttpMethod, StringComparison.Ordinal) == false)
        {
            throw new InsideOutException($"expected http method {method}");
        }
    }

    private void RequireContentType(HttpListenerRequest request, string contentType)
    {
        if(request.ContentType is null) throw new InsideOutException($"no content-type in request");

        var type = new ContentType(request.ContentType);
        if(contentType != type.MediaType) throw new InsideOutException($"content-type must be {contentType}");
    }
}
