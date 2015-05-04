using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Church.Common;
using Arrow.Church.Common.Data;
using Arrow.Church.Common.Net;
using Arrow.Church.Common.ServiceDispatchers;
using Arrow.Threading;

namespace Arrow.Church.Server.ServiceListeners
{
	public class InProcessServiceListener : ServiceListener
	{
		private readonly IWorkDispatcher m_CallDispatcher=new ThreadPoolWorkDispatcher();
		private readonly Uri m_Endpoint;

		private readonly object m_SyncRoot=new object();
		private readonly Dictionary<Tuple<long,long>,DispatcherCallback> m_Callbacks=new Dictionary<Tuple<long,long>,DispatcherCallback>();

		public InProcessServiceListener(Uri endpoint) : base()
		{
			if(InProcessServiceDispatcherRouter.Register(endpoint,RouterCallback)==false)
			{
				throw new ChurchException("endpoint already registered: "+endpoint);
			}

			m_Endpoint=endpoint;
		}

		public override void Respond(CallDetails callDetails, IList<ArraySegment<byte>> buffers)
		{
			var key=Tuple.Create(callDetails.RequestMessageEnvelope.MessageSystemID,callDetails.RequestMessageEnvelope.MessageCorrelationID);
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
				var response=CreateReponse(callDetails.RequestMessageEnvelope);
				response.DataLength=buffers.TotalLength();

				callback(response,buffers);
			}
		}

		private void RouterCallback(MessageEnvelope requestMessageEnvelope, byte[] data, DispatcherCallback dispatcherCallback)
		{
			var key=Tuple.Create(requestMessageEnvelope.MessageSystemID,requestMessageEnvelope.MessageCorrelationID);

			lock(m_SyncRoot)
			{
				m_Callbacks.Add(key,dispatcherCallback);
			}
			
			m_CallDispatcher.QueueUserWorkItem(s=>DispatchCall(requestMessageEnvelope,data));
		}

		private void DispatchCall(MessageEnvelope envelope, byte[] data)
		{
			var callID=AllocateCallID();
			var callDetails=new CallDetails(envelope,data,callID);

			var args=new ServiceCallEventArgs(this,callDetails);
			OnServiceCall(args);
		}

		public override string ToString()
		{
			return m_Endpoint.ToString();
		}
	}
}
