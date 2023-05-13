using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Messaging.Memory
{
	/// <summary>
	/// Manages queue and topic connections
	/// </summary>
	static partial class ConnectionManager
	{
        /// <summary>
        /// Creates the appropriate session
        /// </summary>
        /// <param name="uri">The message uri</param>
        /// <returns></returns>
        public static Session GetSession(Uri uri)
        {
            if(uri == null) throw new ArgumentNullException("uri");

            Session session = null;

            if(uri.Scheme == MemoryScheme.MemoryQueue)
            {
                session = CreateQueueSession(uri);
            }
            else if(uri.Scheme == MemoryScheme.MemoryTopic)
            {
                session = CreateTopicSession(uri);
            }
            else
            {
                throw new MessagingException("unsupported scheme: " + uri.Scheme);
            }

            return session;
        }
    }
}
