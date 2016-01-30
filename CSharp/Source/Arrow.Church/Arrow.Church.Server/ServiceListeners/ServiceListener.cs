using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Arrow.Church.Common.Data;
using Arrow.Church.Common.Net;
using Arrow.Collections;
using Arrow.Threading;

namespace Arrow.Church.Server.ServiceListeners
{
	public abstract class ServiceListener : IDisposable
	{
		private static long s_SystemID;
		
		private readonly long m_SystemID;
		private long m_CorrelationID;

		private long m_CallID;

		private readonly Uri m_Endpoint;

		/// <summary>
		/// Raised when a service call is received
		/// </summary>
		public event EventHandler<ServiceCallEventArgs> ServiceCall;

		protected ServiceListener(Uri endpoint)
		{
			if(endpoint==null) throw new ArgumentNullException("endpoint");
			m_Endpoint=endpoint;

			m_SystemID=Interlocked.Increment(ref s_SystemID);
		}

		public Uri Endpoint
		{
			get{return m_Endpoint;}
		}

		public abstract void Start();

		public abstract void Stop();

		/// <summary>
		/// Sends a response back to the sender
		/// </summary>
		/// <param name="callDetails"></param>
		/// <param name="buffers"></param>
		/// <returns>A task that is compete when the response has been sent</returns>
		public abstract Task RespondAsync(CallDetails callDetails, ArraySegmentCollection<byte> buffers);

		protected void OnServiceCall(ServiceCallEventArgs args)
		{
			var d=ServiceCall;
			if(d!=null) d(this,args);
		}

		/// <summary>
		/// The unique ID for this listener
		/// </summary>
		public long SystemID
		{
			get{return m_SystemID;}
		}

		protected long AllocateCorrelationID()
		{
			return Interlocked.Increment(ref m_CorrelationID);
		}

		protected long AllocateCallID()
		{
			return Interlocked.Increment(ref m_CallID);
		}

		protected MessageEnvelope CreateReponse(MessageEnvelope template)
		{
			var response=new MessageEnvelope();

			response.MessageSystemID=m_SystemID;
			response.MessageCorrelationID=AllocateCorrelationID();
			response.ResponseSystemID=template.MessageSystemID;
			response.ResponseCorrelationID=template.MessageCorrelationID;
			response.SessionHigh=template.SessionHigh;
			response.SessionLow=template.SessionLow;

			response.MessageType=template.MessageType|1;

			return response;
		}

		protected void HandleMessage(CallDetails callDetails)
		{
			switch(callDetails.Envelope.MessageType)
			{
				case MessageType.ServiceRequest:
					HandleServiceRequest(callDetails);
					break;

				case MessageType.Ping:
					HandlePing(callDetails);
					break;

				default:
					// TODO
					break;
			}
		}

		protected virtual void HandleServiceRequest(CallDetails callDetails)
		{
			var args=new ServiceCallEventArgs(this,callDetails);
			OnServiceCall(args);
		}

		protected virtual void HandlePing(CallDetails callDetails)
		{
		}

		public virtual void Dispose()
		{
			// Does nothing
		}

		public override string ToString()
		{
			return m_Endpoint.ToString();
		}
	}
}
