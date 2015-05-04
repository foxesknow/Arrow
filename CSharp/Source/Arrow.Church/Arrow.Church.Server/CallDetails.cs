using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Church.Common.Net;

namespace Arrow.Church.Server
{
	public sealed class CallDetails
	{
		private readonly MessageEnvelope m_RequestMessageEnvelope;
		private readonly byte[] m_Data;
		private readonly long m_CallID;

		public CallDetails(MessageEnvelope requestMessageEnvelope, byte[] data, long callID)
		{
			m_RequestMessageEnvelope=requestMessageEnvelope;
			m_Data=data;
			m_CallID=callID;
		}

		public MessageEnvelope RequestMessageEnvelope
		{
			get{return m_RequestMessageEnvelope;}
		}

		public byte[] Data
		{
			get{return m_Data;}
		}

		public long CallID
		{
			get{return m_CallID;}
		}
	}
}
