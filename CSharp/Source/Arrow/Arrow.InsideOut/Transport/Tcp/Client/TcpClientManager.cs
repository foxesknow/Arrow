using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Threading.Tasks;
using Arrow.InsideOut.Transport.Implementation;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using Arrow.Threading;

namespace Arrow.InsideOut.Transport.Tcp.Client;

/// <summary>
/// Manages connections to a server that are exposed via tcp.
/// 
/// The endpoint used to connect must have the for tcp://host:port
/// </summary>
public sealed partial class TcpClientManager : ClientManagerBase, IClientManager, IDisposable
{
    private readonly InsideOutEncoder m_Encoder = InsideOutEncoder.Default;

    private readonly Func<INetworkClient> m_NetworkClientFactory;

    private readonly CancelSource m_CancelSource = new();
    private readonly ConcurrentDictionary<PublisherID, (string Host, int Port)> m_Publishers = new();

    public TcpClientManager() : this(MakeNetworkClient)
    {
    }

    internal TcpClientManager(Func<INetworkClient> networkClientFactory)
    {
        ArgumentNullException.ThrowIfNull(networkClientFactory);

        m_NetworkClientFactory = networkClientFactory;
    }

    public override void Dispose()
    {
        if(this.IsDisposed == false)
        {
            m_CancelSource.Cancel();
            base.Dispose();
        }
    }

    public IInsideOutNode Register(PublisherID publisherID, Uri endpoint)
    {
        ArgumentNullException.ThrowIfNull(publisherID);
        ArgumentNullException.ThrowIfNull(endpoint);
        ThrowIfDisposed();

        var details = ExtractHostAndPort(endpoint);
        if(m_Publishers.TryAdd(publisherID, details))
        {
            return new Proxy(this, publisherID, details.Host, details.Port);
        }

        throw new InsideOutException($"publisher already registered: {publisherID}");
    }

    public bool IsRegistered(PublisherID publisherID)
    {
        ArgumentNullException.ThrowIfNull(publisherID);

        return m_Publishers.ContainsKey(publisherID);
    }

    public bool TryGetNode(PublisherID publisherID, [NotNullWhen(true)] out IInsideOutNode? node)
    {
        ArgumentNullException.ThrowIfNull(publisherID);
        ThrowIfDisposed();

        if(m_Publishers.TryGetValue(publisherID, out var details))
        {
            node = new Proxy(this, publisherID, details.Host, details.Port);
            return true;
        }

        node = null;
        return false;
    }

    private async ValueTask<ExecuteResponse> Execute(PublisherID publisherID, string host, int port, CancellationToken ct, ExecuteRequest request)
    {
        ThrowIfDisposed();

        var requestID = this.RequestIDFactory.Make();
        var transportRequest = new TransportRequest(NodeFunction.Execute, publisherID, requestID)
        {
            Request = request
        };

        using(var client = m_NetworkClientFactory())
        {
            await client.Connect(host, port).ContinueOnAnyContext();

            using(CombineCancellationToken.Make(ct, m_CancelSource.Token, out var cancellationToken))
            using(var stream = client.GetStream())
            {
                await Send(publisherID, transportRequest, stream, cancellationToken).ContinueOnAnyContext();
                var response = await Receive(stream, cancellationToken);

                return ReturnExpected<ExecuteResponse>(response);
            }
        }
    }

    private async ValueTask<Details> GetDetails(PublisherID publisherID, string host, int port, CancellationToken ct)
    {
        ThrowIfDisposed();

        var requestID = this.RequestIDFactory.Make();
        var transportRequest = new TransportRequest(NodeFunction.GetDetails, publisherID, requestID);

        using(var client = m_NetworkClientFactory())
        {
            await client.Connect(host, port).ContinueOnAnyContext();

            using(CombineCancellationToken.Make(ct, m_CancelSource.Token, out var cancellationToken))
            using(var stream = client.GetStream())
            {
                await Send(publisherID, transportRequest, stream, cancellationToken).ContinueOnAnyContext();
                var response = await Receive(stream, cancellationToken);

                return ReturnExpected<Details>(response);
            }
        }
    }

    private async ValueTask Send(PublisherID publisherID, TransportRequest request, Stream stream, CancellationToken ct)
    {
        using(var d = m_Encoder.EncodeToPool(request))
        {
            if(d.HasBuffer == false) throw new IOException("no buffer!");

            await StreamSupport.Write(stream, d.Buffer, 0, d.Length, ct).ContinueOnAnyContext();
        }
    }

    private async ValueTask<object?> Receive(Stream stream, CancellationToken ct)
    {
        using(var r = await StreamSupport.Read(stream, ct))
        {
            if(r.HasBuffer == false) throw new IOException("no buffer received");

            var response = m_Encoder.Decode<TransportResponse>(r.Buffer, 0, r.Length);
            return ExtractResponseData(response);
        }
    }

    internal static (string Host, int Port) ExtractHostAndPort(Uri endpoint)
    {
        if(endpoint.Scheme != "tcp") throw new IOException($"not a tcp endpoint - {endpoint}");

        var host = endpoint.Host;
        if(string.IsNullOrWhiteSpace(host)) throw new ArgumentException("invalid host in endpoint", nameof(endpoint));

        var port = endpoint.Port;
        if(port < 0 || port > 65535) throw new ArgumentException("invalid port in endpoint", nameof(endpoint));

        return (host, port);
    }

    private static INetworkClient MakeNetworkClient()
    {
        return new TcpNetworkClient();
    }
}
