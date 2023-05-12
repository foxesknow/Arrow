using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Arrow.Execution;
using Arrow.InsideOut.Transport.Implementation;
using Arrow.IO;
using Arrow.Logging;
using Arrow.Threading.Tasks;

namespace Arrow.InsideOut.Transport.Tcp.Server;

public sealed class ListenerPlugin : ListenerPluginBase
{
    private const string ListenerName = "InsideOut.Tcp.ListenerPlugin";

    private static readonly ILog Log = new PrefixLog(LogManager.GetDefaultLog(), $"[{ListenerName}]");

    private readonly CancelSource m_CancelSource = new();

    private readonly SequentialWorkQueue m_WorkQueue = new();

    private Task? m_ListenTask;

    /// <summary>
    /// The port to listen on
    /// </summary>
    public int Port{get; init;}

    public override string Name => ListenerName;

    protected override ValueTask Start()
    {
        if(this.InstanceName is null) throw new InsideOutException("no instance name specified");
        if(this.Port < 0 || this.Port > 65535) throw new InsideOutException($"invalid port: {this.Port}");

        var publisherID = new PublisherID(Environment.MachineName, this.InstanceName);
        Register(publisherID);

        m_ListenTask = Task.Run(() => Listen(m_CancelSource.CanceledTask, m_CancelSource.Token));

        return default;
    }

    protected override async ValueTask Stop()
    {
        m_CancelSource.Cancel();

        if(m_ListenTask is not null)
        {
            await m_ListenTask.ContinueOnAnyContext();
            m_ListenTask = null;
        }

        m_CancelSource.Dispose();
        m_WorkQueue.Dispose();
    }

    private async void Listen(Task cancelTask, CancellationToken ct)
    {
        var listener = new TcpListener(IPAddress.Any, this.Port);

        try
        {
            listener.Start();

            while(true)
            {
                var acceptTask = listener.AcceptTcpClientAsync(ct).AsTask();
                var task = await Task.WhenAny(cancelTask, acceptTask).ContinueOnAnyContext();

                if(task != acceptTask) break;

                var client = await acceptTask.ContinueOnAnyContext();
                if(m_WorkQueue.TryEnqueue(() => HandleClient(client, ct)) == false)
                {
                    client.Close();
                }
            }

            Log.Info("stopping listening");
        }
        catch(OperationCanceledException e) when (e.CancellationToken == ct)
        {
            Log.Info("stopping listening");
        }
        catch(OperationCanceledException e)
        {
            Log.Error("unexpected cancellation", e);
        }
        catch(Exception e)
        {
            Log.Error("listening failed", e);
        }
        finally
        {
            MethodCall.AllowFail(listener, static listener => listener.Stop());
        }
    }

    private async ValueTask HandleClient(TcpClient client, CancellationToken ct)
    {
        try
        {
            using(client)
            using(var stream = client.GetStream())
            using(var releaser = await StreamSupport.Read(stream, ct).ContinueOnAnyContext())
            {
                if(releaser.HasBuffer == false) throw new IOException("no buffer read");

                var (success, response) = await Process(releaser.Buffer, 0, releaser.Length, ct);
                if(success == false || response is null) return;

                using(var poolStream = new ArrayPoolMemoryStream())
                {
                    this.Encoder.Encode(response, poolStream);

                    using(var d  = poolStream.Detach())
                    {
                        await StreamSupport.Write(stream, d.Buffer!, 0, d.Length, ct).ContinueOnAnyContext();
                    }
                }
            }
        }
        catch(Exception e)
        {
            Log.Error("error handling client request", e);
        }
    }
}
