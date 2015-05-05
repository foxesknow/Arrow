using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Church.Common.Data;
using Arrow.Church.Common.Data.DotNet;
using Arrow.Church.Common.Net;

using Arrow.Threading;
using Arrow.Logging;

namespace Arrow.Church.Server
{
	public class ServiceHost : IDisposable
	{
		private static readonly MessageProtocol s_DotNetSerializer=new SerializationMessageProtocol();
		private static readonly ILog Log=LogManager.GetDefaultLog();

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

		/// <summary>
		/// Starts all the services
		/// </summary>
		public void Start()
		{
			Log.Info("ServiceHost.Start - starting");
			m_ServiceContainer.Start();
			m_ServiceListener.Start();
			Log.Info("ServiceHost.Start - started");
		}

		/// <summary>
		/// Stops all the services
		/// </summary>
		public void Stop()
		{
			Log.Info("ServiceHost.Stop - stopping");
			m_ServiceListener.Stop();
			m_ServiceContainer.Stop();
			Log.Info("ServiceHost.Stop - stopped");
		}

		/// <summary>
		/// Called when a service call must be processed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void HandleServiceCall(object sender, ServiceCallEventArgs args)
		{
			m_ServiceCallRequestQueue.Enqueue(()=>ProcessCallRequest(args));
		}

		private void ProcessCallRequest(ServiceCallEventArgs args)
		{
			using(var stream=new MemoryStream(args.CallDetails.Data))
			{
				// First up, work out what we need to call...
				ServiceCallRequest callDetails=null;

				using(var decoder=new DataDecoder(stream))
				{
					callDetails=decoder.ReadEncodedDataNeverNull(d=>new ServiceCallRequest(d));
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

						m_CallDispatcher.QueueUserWorkItem((state)=>ExecuteCall(callDetails,message,args,serviceData));
					}
				}
			}			
		}

		/// <summary>
		/// Executes a service call and 
		/// </summary>
		/// <param name="callDetails"></param>
		/// <param name="message"></param>
		/// <param name="args"></param>
		/// <param name="serviceData"></param>
		private void ExecuteCall(ServiceCallRequest callDetails, object message, ServiceCallEventArgs args, ServiceData serviceData)
		{
			var task=m_ServiceContainer.Execute(callDetails.ServiceName,callDetails.ServiceMethod,message);
			task.ContinueWith(t=>AfterServiceCall(t,callDetails,args,serviceData));
		}

		/// <summary>
		/// Called after the service method has executed.
		/// Packages up the return value and schedules it for sending back to the client
		/// </summary>
		/// <param name="call"></param>
		/// <param name="callDetails"></param>
		/// <param name="args"></param>
		/// <param name="serviceData"></param>
		private void AfterServiceCall(Task<object> call, ServiceCallRequest callDetails, ServiceCallEventArgs args, ServiceData serviceData)
		{
			var response=new ServiceCallResponse(callDetails.ServiceName,callDetails.ServiceMethod,call.IsFaulted);

			using(var stream=new MemoryStream())
			{
				using(var encoder=new DataEncoder(stream))
				{
					encoder.WriteNeverNull(response);
				}

				var protocol=serviceData.Service.MessageProtocol;

				if(call.IsFaulted)
				{
					// Exceptions always go back as a .NET serialized stream 
					// as some message formats dont support exceptions
					s_DotNetSerializer.ToStream(stream,call.Exception);
				}
				else
				{
					protocol.ToStream(stream,call.Result);
				}

				var segments=stream.ToArraySegment().ToList();
				args.ServiceListener.Respond(args.CallDetails,segments);
			}
		}

		public void Dispose()
		{
			Stop();
		}
	}
}
