using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Church.Common.Data;
using Arrow.Threading;

namespace Arrow.Church.Server
{
	public class ServiceHost
	{
		private readonly ServiceListener m_ServiceListener;
		private readonly MessageProtocol m_MessageProtocol;
		private readonly ServiceContainer m_ServiceContainer=new ServiceContainer();

		private readonly ActionWorkQueue m_ServiceCallRequestQueue=new ActionWorkQueue();
		private readonly IWorkDispatcher m_CallDispatcher=new ThreadPoolWorkDispatcher();

		public ServiceHost(ServiceListener serviceListener, MessageProtocol messageProtocol)
		{
			if(serviceListener==null) throw new ArgumentNullException("serviceListener");
			if(messageProtocol==null) throw new ArgumentNullException("messageProtocol");

			m_ServiceListener=serviceListener;
			m_MessageProtocol=messageProtocol;

			m_ServiceListener.ServiceCall+=HandleServiceCall;
		}

		public ServiceContainer ServiceContainer
		{
			get{return m_ServiceContainer;}
		}

		private void HandleServiceCall(object sender, ServiceCallEventArgs args)
		{
			m_ServiceCallRequestQueue.Enqueue(()=>ProcessCallRequest(args));
		}

		private void ProcessCallRequest(ServiceCallEventArgs args)
		{
			using(var stream=new MemoryStream(args.Data))
			{
				// First up, work out what we need to call...
				ServiceCallRequest callDetails=null;

				using(var decoder=new DataDecoder(stream))
				{
					callDetails=decoder.ReadEncodedData(d=>new ServiceCallRequest(d));
				}

				// ...then the actual message to the service
				object message=m_MessageProtocol.FromStream(stream);

				m_CallDispatcher.QueueUserWorkItem((state)=>DispatchCall(callDetails,message,args));
			}			
		}

		private void DispatchCall(ServiceCallRequest callDetails, object message, ServiceCallEventArgs args)
		{
			var task=m_ServiceContainer.Execute(callDetails.ServiceName,callDetails.ServiceMethod,message);
			task.ContinueWith(t=>AfterServiceCall(t,callDetails,args));
		}

		private void AfterServiceCall(Task<object> call, ServiceCallRequest callDetails, ServiceCallEventArgs args)
		{
			var response=new ServiceCallResponse(callDetails.ServiceName,callDetails.ServiceMethod,call.IsFaulted);

			byte[] responseBuffer=null;

			using(var stream=new MemoryStream())
			{
				using(var encoder=new DataEncoder(stream))
				{
					encoder.Write(response);
				}

				if(call.IsFaulted)
				{
					m_MessageProtocol.ToStream(stream,call.Exception);
				}
				else
				{
					m_MessageProtocol.ToStream(stream,call.Result);
				}

				responseBuffer=stream.ToArray();
			}

			var segment=new ArraySegment<byte>(responseBuffer);
			var segments=segment.ToList();
			args.ServiceListener.Respond(args.SenderMessageEnvelope,segments);
		}
	}
}
