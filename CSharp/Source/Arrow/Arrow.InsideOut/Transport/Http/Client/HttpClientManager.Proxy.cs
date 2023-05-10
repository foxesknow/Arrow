using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.InsideOut.Transport.Http.Client;

public sealed partial class HttpClientManager
{
    /// <summary>
    /// The proxy looks like an InsideOut node, but uses http to make calls to the actual server.
    /// </summary>
    private sealed class Proxy : IInsideOutNode
    {
        private readonly HttpClientManager m_ClientManager;
        private readonly PublisherID m_PublisherID;
        private readonly Uri m_Endpoint;
        private readonly Guid m_ApplicationID;

        public Proxy(HttpClientManager clientManager, PublisherID publisherID, Uri endpoint, Guid applicationID)
        {
            m_ClientManager = clientManager;
            m_PublisherID = publisherID;
            m_Endpoint = endpoint;
            m_ApplicationID = applicationID;
        }

        public ValueTask<ExecuteResponse> Execute(ExecuteRequest request, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(request);
            return m_ClientManager.Execute(m_PublisherID, m_Endpoint, m_ApplicationID, ct, request);
        }

        public ValueTask<Details> GetDetails(CancellationToken ct)
        {
            return m_ClientManager.GetDetails(m_PublisherID, m_Endpoint, m_ApplicationID, ct);
        }

        public override string ToString()
        {
            return m_Endpoint.ToString();
        }
    }
}
