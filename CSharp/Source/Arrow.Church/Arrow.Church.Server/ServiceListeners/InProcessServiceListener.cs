using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Church.Common;
using Arrow.Church.Common.Data;
using Arrow.Church.Common.Net;
using Arrow.Church.Common.ServiceDispatchers;
using Arrow.Memory;
using Arrow.Threading;

namespace Arrow.Church.Server.ServiceListeners
{
	public class InProcessServiceListener : ServiceListener
	{
		private readonly IWorkDispatcher m_CallDispatcher=new ThreadPoolWorkDispatcher();

		private readonly object m_SyncRoot=new object();
		private readonly Dictionary<Tuple<long,long>,DispatcherCallback> m_Callbacks=new Dictionary<Tuple<long,long>,DispatcherCallback>();

		public InProcessServiceListener(Uri endpoint) : base(endpoint)
		{
			if(InProcessServiceDispatcherRouter.Register(endpoint,RouterCallback)==false)
			{
				throw new ChurchException("endpoint already registered: "+endpoint);
			}
		}

		public override void Start()
		{
			// Does nothing
		}

		public override void Stop()
		{
			// Does nothing
		}

		public override Task RespondAsync(CallDetails callDetails, ArraySegmentCollection<byte> buffers)
		{
			try
			{
				var key=Tuple.Create(callDetails.Envelope.MessageSystemID,callDetails.Envelope.MessageCorrelationID);
				DispatcherCallback callback=null;

				lock(m_SyncRoot)
				{
					if(m_Callbacks.TryGetValue(key,out callback))
					{
						m_Callbacks.Remove(key);
					}
				}

				if(callback!=null)
				{
					var response=CreateReponse(callDetails.Envelope);
					response.DataLength=buffers.GetOverallLength();

					callback(response,buffers);
				}

				return Task.FromResult(true);
			}
			catch(Exception e)
			{
				return TaskEx.FromException(e);
			}
		}

		private void RouterCallback(MessageEnvelope requestMessageEnvelope, byte[] data, DispatcherCallback dispatcherCallback)
		{
			var key=Tuple.Create(requestMessageEnvelope.MessageSystemID,requestMessageEnvelope.MessageCorrelationID);

			lock(m_SyncRoot)
			{
				m_Callbacks.Add(key,dispatcherCallback);
			}
			
			var callID=AllocateCallID();
			var callDetails=new CallDetails(requestMessageEnvelope,data,callID);
			m_CallDispatcher.QueueUserWorkItem(s=>HandleMessage(callDetails));
		}		
	}
}
