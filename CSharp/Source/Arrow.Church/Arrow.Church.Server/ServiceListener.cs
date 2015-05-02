﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Arrow.Church.Common.Data;
using Arrow.Church.Common.Net;
using Arrow.Threading;

namespace Arrow.Church.Server
{
	public abstract class ServiceListener : IDisposable
	{
		private static long s_SystemID;
		
		private readonly long m_SystemID;
		private long m_CorrelationID;

		/// <summary>
		/// Raised when a service call is received
		/// </summary>
		public event EventHandler<ServiceCallEventArgs> ServiceCall;

		protected ServiceListener()
		{
			m_SystemID=Interlocked.Increment(ref s_SystemID);
		}

		/// <summary>
		/// Sends a response back to the sender
		/// </summary>
		/// <param name="senderMessageEnvelope"></param>
		/// <param name="buffers"></param>
		public abstract void Respond(MessageEnvelope requestMessageEnvelope, IList<ArraySegment<byte>> buffers);

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

		protected MessageEnvelope CreateReponse(MessageEnvelope template)
		{
			var response=new MessageEnvelope();

			response.MessageSystemID=m_SystemID;
			response.MessageCorrelationID=AllocateCorrelationID();
			response.ResponseSystemID=template.MessageSystemID;
			response.ResponseCorrelationID=template.MessageCorrelationID;
			response.MessageType=MessageType.Response;

			return response;
		}

		public virtual void Dispose()
		{
			// Does nothing
		}
	}
}
