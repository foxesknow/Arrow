using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Arrow.InsideOut.Transport.Implementation;
using Arrow.Threading;
using Arrow.Threading.Tasks;

namespace Arrow.InsideOut.Transport.Http.Client;

/// <summary>
/// Manages connections to servers that are using http
/// </summary>
public sealed partial class HttpClientManager : ClientManagerBase, IClientManager, IDisposable
{
    private readonly HttpClient m_HttpClient;
    private readonly InsideOutJsonSerializer m_Serializer = new();

    private readonly ConcurrentDictionary<PublisherID, (Uri Endpoint, Guid ApplicationID)> m_Publishers = new();
    private readonly CancellationTokenSource m_Cts = new();

    public HttpClientManager()
    {
        // We'll let the client do the decompression for us
        var handler = new HttpClientHandler()
        {
            AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
        };

        m_HttpClient = new(handler);
    }
    
    /// <inheritdoc/>
    public override void Dispose()
    {
        if(this.IsDisposed == false)
        {
            base.Dispose();

            m_Cts.Cancel();
            m_HttpClient.Dispose();
            m_Cts.Dispose();
        }
    }

    /// <summary>
    /// Registers to an endpoint
    /// </summary>
    /// <param name="publisherID"></param>
    /// <param name="endpoint"></param>
    /// <returns></returns>
    public IInsideOutNode Register(PublisherID publisherID, Uri endpoint)
    {
        return Register(publisherID, endpoint, this.RequestIDFactory.ApplicationID);
    }

    /// <summary>
    /// Registers to an endpoint using a specific application id
    /// </summary>
    /// <param name="publisherID"></param>
    /// <param name="endpoint"></param>
    /// <param name="applicationID"></param>
    /// <returns></returns>
    /// <exception cref="InsideOutException"></exception>
    public IInsideOutNode Register(PublisherID publisherID, Uri endpoint, Guid applicationID)
    {
        ArgumentNullException.ThrowIfNull(publisherID);
        ArgumentNullException.ThrowIfNull(endpoint);
        ThrowIfDisposed();

        var processEndpoint = MakeProcessEndpoint(endpoint);
        if(m_Publishers.TryAdd(publisherID, (processEndpoint, applicationID)))
        {
            return new Proxy(this, publisherID, processEndpoint, applicationID);
        }

        throw new InsideOutException($"publisher already registered: {publisherID}");
    }

    /// <inheritdoc/>
    public bool IsRegistered(PublisherID publisherID)
    {
        ArgumentNullException.ThrowIfNull(publisherID);
        return m_Publishers.ContainsKey(publisherID);
    }

    /// <inheritdoc/>
    public bool TryGetNode(PublisherID publisherID, [NotNullWhen(true)] out IInsideOutNode? node)
    {
        ArgumentNullException.ThrowIfNull(publisherID);
        ThrowIfDisposed();

        if(m_Publishers.TryGetValue(publisherID, out var details))
        {
            node = new Proxy(this, publisherID, details.Endpoint, details.ApplicationID);
            return true;
        }

        node = null;
        return false;
    }

    private Uri MakeProcessEndpoint(Uri endpoint)
    {
        var builder = new UriBuilder(endpoint);
        if(builder.Path.EndsWith('/') == false) builder.Path += '/';

        builder.Path += "Process";
        return builder.Uri;
    }

    private async ValueTask<ExecuteResponse> Execute(PublisherID publisherID,  Uri endpoint, Guid applicationID, CancellationToken ct, ExecuteRequest request)
    {
        ThrowIfDisposed();

        var requestID = this.RequestIDFactory.Make(applicationID);
        var transportRequest = new TransportRequest(NodeFunction.Execute, publisherID, requestID)
        {
            Request = request
        };

        var response = await Send(transportRequest, publisherID, endpoint, ct).ContinueOnAnyContext();
        return ReturnExpected<ExecuteResponse>(response);
    }

    private async ValueTask<NodeDetails> GetDetails(PublisherID publisherID,  Uri endpoint, Guid applicationID, CancellationToken ct)
    {
        ThrowIfDisposed();

        var requestID = this.RequestIDFactory.Make(applicationID);
        var transportRequest = new TransportRequest(NodeFunction.GetDetails, publisherID, requestID);

        var response = await Send(transportRequest, publisherID, endpoint, ct).ContinueOnAnyContext();
        return ReturnExpected<NodeDetails>(response);
    }

    private async ValueTask<object?> Send(TransportRequest transportRequest, PublisherID publisherID, Uri endpoint, CancellationToken ct)
    {
        var json = m_Serializer.Serialize(transportRequest);

        using(var content = new StringContent(json, Encoding.UTF8, MimeType.Json))
        using(CombineCancellationToken.Make(ct, m_Cts.Token, out var cancellationToken))
        using(var response = await m_HttpClient.PostAsync(endpoint, content, cancellationToken).ContinueOnAnyContext())
        {
            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jsonReponse = await response.Content.ReadAsStringAsync().ContinueOnAnyContext();
                return ToReponseData(jsonReponse);
            }

            // We've got an http error code
            var error = await ExtractError(response).ContinueOnAnyContext();
            throw new InsideOutException($"StatusCode = {response.StatusCode}, Error = {error}");
        }
    }

    private async ValueTask<string> ExtractError(HttpResponseMessage message)
    {
        try
        {
            return await message.Content.ReadAsStringAsync().ContinueOnAnyContext();
        }
        catch
        {
            return "no reason returned from server";
        }
    }

    private object? ToReponseData(string? json)
    {
        if(json is null) throw new InsideOutException("no json in response");

        var response = m_Serializer.Deserialize<TransportResponse>(json);
        return ExtractResponseData(response);
    }
}
