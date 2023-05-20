using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.InsideOut.Transport.Tcp.Client;

public partial class TcpClientManager
{
    private sealed class Proxy : IInsideOutNode
    {
        private readonly TcpClientManager m_ClientManager;
        private readonly string m_Host;
        private readonly int m_Port;
        private readonly PublisherID m_PublisherID;

        public Proxy(TcpClientManager clientManager, PublisherID publisherID, string host, int port)
        {
            m_ClientManager = clientManager;
            m_PublisherID = publisherID;
            m_Host = host;
            m_Port = port;
        }

        public ValueTask<ExecuteResponse> Execute(ExecuteRequest request, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(request);
            return m_ClientManager.Execute(m_PublisherID, m_Host, m_Port, ct, request);
        }

        public ValueTask<NodeDetails> GetDetails(CancellationToken ct)
        {
            return m_ClientManager.GetDetails(m_PublisherID, m_Host, m_Port, ct);
        }
    }
}
