using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Church.Common.Net;

namespace Arrow.Church.Server
{
	public class ServiceCallEventArgs : EventArgs
	{
		private readonly ServiceListener m_ServiceListener;
		private readonly MessageEnvelope m_SenderMessageEnvelope;
		private readonly byte[] m_Data;

		public ServiceCallEventArgs(ServiceListener serviceListener, MessageEnvelope senderMessageEnvelope, byte[] data)
		{
			m_ServiceListener=serviceListener;
			m_SenderMessageEnvelope=senderMessageEnvelope;
			m_Data=data;
		}

		public ServiceListener ServiceListener
		{
			get{return m_ServiceListener;}
		}

		public MessageEnvelope SenderMessageEnvelope
		{
			get{return m_SenderMessageEnvelope;}
		}

		public byte[] Data
		{
			get{return m_Data;}
		}
	}
}
