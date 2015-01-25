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
		private static readonly object s_TopicLock=new object();
		private static readonly Dictionary<Uri,TopicSession> s_TopicSessions=new Dictionary<Uri,TopicSession>();

		private static TopicSession CreateTopicSession(Uri topicUri)
		{
			if(topicUri==null) throw new ArgumentNullException("topicUri");
			if(topicUri.Scheme!=MemoryScheme.MemoryTopic) throw new MessagingException("invalid memory scheme: "+topicUri.Scheme);

			var session=GetTopicSession(topicUri);
			session.AddRef();

			return session;
		}

		private static TopicSession GetTopicSession(Uri address)
		{
			lock(s_TopicLock)
			{
				TopicSession session=null;
				if(s_TopicSessions.TryGetValue(address,out session)==false)
				{
					session=new TopicSession(address);
					s_TopicSessions.Add(address,session);
				}

				return session;
			}
		}
	}
}
