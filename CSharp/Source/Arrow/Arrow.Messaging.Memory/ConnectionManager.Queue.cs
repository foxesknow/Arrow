using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Collections;
using Arrow.Collections.Extensions;
using Arrow.Threading;

namespace Arrow.Messaging.Memory
{
	static partial class ConnectionManager
	{
        private static readonly object s_QueueLock = new object();
        private static readonly Dictionary<Uri, QueueSession> s_QueueSessions = new Dictionary<Uri, QueueSession>();

        /// <summary>
        /// Creates a queue session
        /// </summary>
        /// <param name="queueUri"></param>
        /// <returns></returns>
        private static QueueSession CreateQueueSession(Uri queueUri)
        {
            if(queueUri == null) throw new ArgumentNullException("queueUri");
            if(queueUri.Scheme != MemoryScheme.MemoryQueue) throw new MessagingException("invalid memory scheme: " + queueUri.Scheme);

            var session = GetQueueSession(queueUri);
            session.AddRef();

            return session;
        }

        private static QueueSession GetQueueSession(Uri address)
        {
            lock(s_QueueLock)
            {
                QueueSession session = null;
                if(s_QueueSessions.TryGetValue(address, out session) == false)
                {
                    session = new QueueSession(address);
                    s_QueueSessions.Add(address, session);
                }

                return session;
            }
        }
    }
}
