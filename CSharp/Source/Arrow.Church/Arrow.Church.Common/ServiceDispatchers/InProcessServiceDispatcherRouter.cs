using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Church.Common.Net;
using Arrow.Threading;

namespace Arrow.Church.Common.ServiceDispatchers
{
	public static class InProcessServiceDispatcherRouter
	{
		private static readonly object s_SyncRoot=new object();
		private static readonly ActionWorkQueue s_Queue=new ActionWorkQueue();
		private static readonly Dictionary<Uri,ListenerCallback> s_Callbacks=new Dictionary<Uri,ListenerCallback>();

		public static void Enqueue(Uri endpoint, MessageEnvelope envelope, byte[] data, DispatcherCallback dispatcherCallback)
		{
			s_Queue.Enqueue(()=>
			{
				var callback=GetCallback(endpoint);
				if(callback!=null)
				{
					callback(envelope,data,dispatcherCallback);
				}
			});
		}

		public static bool Register(Uri endpoint, ListenerCallback callback)
		{
			lock(s_SyncRoot)
			{
				if(s_Callbacks.ContainsKey(endpoint))
				{
					return false;
				}
				else
				{
					s_Callbacks.Add(endpoint,callback);
					return true;
				}
			}
		}

		public static void Unregister(Uri endpoint)
		{
			// Does nothing
		}

		private static ListenerCallback GetCallback(Uri endpoint)
		{
			lock(s_SyncRoot)
			{
				ListenerCallback callback=null;
				s_Callbacks.TryGetValue(endpoint,out callback);

				return callback;
			}
		}
	}
}
