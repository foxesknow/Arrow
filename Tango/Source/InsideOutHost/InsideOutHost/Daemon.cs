using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Application.DaemonHosting;
using Arrow.InsideOut.Transport;
using Arrow.InsideOut.Transport.Messaging.Client;

namespace InsideOutHost
{
    internal class Daemon : DaemonBase
    {
        private MessagingBroadcastManager m_BroadcastManager = new();

        protected override ValueTask StartDaemon(string[] args)
        {
            var publisherID = new PublisherID("fuji", "InsideOutHost");
            var endpoint = new Uri("memtopic://foo/InsideOutHost");

            m_BroadcastManager.Subscribe(publisherID, endpoint, HandleBroadcast);

            return default;
        }

        protected override ValueTask StopDaemon()
        {
            m_BroadcastManager.Dispose();
            return default;
        }

        private ValueTask HandleBroadcast(BroadcastData data)
        {
            return default;
        }
    }
}
