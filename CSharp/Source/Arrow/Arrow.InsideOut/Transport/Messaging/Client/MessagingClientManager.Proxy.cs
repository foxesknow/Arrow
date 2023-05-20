using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.InsideOut.Transport.Messaging.Client;

public partial class MessagingClientManager
{
    private class Proxy : IInsideOutNode
    {
        private readonly MessagingClientManager m_ClientManager;
        private readonly PublisherID m_PublisherID;

        public Proxy(MessagingClientManager clientManager, PublisherID publisherID)
        {
            m_ClientManager = clientManager;
            m_PublisherID = publisherID;
        }

        public ValueTask<ExecuteResponse> Execute(ExecuteRequest request, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(request);
            return m_ClientManager.Execute(m_PublisherID, request, ct);
        }

        public ValueTask<NodeDetails> GetDetails(CancellationToken ct)
        {
            return m_ClientManager.GetDetails(m_PublisherID, ct);
        }
    }
}
