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
		private readonly ServiceContainer m_ServiceContainer=new ServiceContainer();

		private readonly ActionWorkQueue m_ServiceCallRequestQueue=new ActionWorkQueue();
		private readonly IWorkDispatcher m_CallDispatcher=new ThreadPoolWorkDispatcher();

		public ServiceHost(ServiceListener serviceListener)
		{
			if(serviceListener==null) throw new ArgumentNullException("serviceListener");

			m_ServiceListener=serviceListener;
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

				ServiceData serviceData;

				if(m_ServiceContainer.TryGetServiceData(callDetails.ServiceName,out serviceData))
				{
					var protocol=serviceData.Service.MessageProtocol;

					// ...then the actual message to the service
					Type parameterType;
					if(serviceData.TryGetParameterType(callDetails.ServiceMethod,out parameterType))
					{
						object message=protocol.FromStream(stream,parameterType);

						m_CallDispatcher.QueueUserWorkItem((state)=>DispatchCall(callDetails,message,args,serviceData));
					}
				}
			}			
		}

		private void DispatchCall(ServiceCallRequest callDetails, object message, ServiceCallEventArgs args, ServiceData serviceData)
		{
			var task=m_ServiceContainer.Execute(callDetails.ServiceName,callDetails.ServiceMethod,message);
			task.ContinueWith(t=>AfterServiceCall(t,callDetails,args,serviceData));
		}

		private void AfterServiceCall(Task<object> call, ServiceCallRequest callDetails, ServiceCallEventArgs args, ServiceData serviceData)
		{
			var response=new ServiceCallResponse(callDetails.ServiceName,callDetails.ServiceMethod,call.IsFaulted);

			byte[] responseBuffer=null;

			using(var stream=new MemoryStream())
			{
				using(var encoder=new DataEncoder(stream))
				{
					encoder.Write(response);
				}

				var protocol=serviceData.Service.MessageProtocol;

				if(call.IsFaulted)
				{
					protocol.ToStream(stream,call.Exception);
				}
				else
				{
					protocol.ToStream(stream,call.Result);
				}

				responseBuffer=stream.ToArray();
			}

			var segment=new ArraySegment<byte>(responseBuffer);
			var segments=segment.ToList();
			args.ServiceListener.Respond(args.SenderMessageEnvelope,segments);
		}
	}
}
