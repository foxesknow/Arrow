using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Church.Common.Data;
using Arrow.Church.Common.Net;
using Arrow.Memory.Pools;
using Arrow.Net.Message;

namespace Arrow.Church.Server.ServiceListeners
{
	class NetworkServiceMessageFactory : IMessageFactory<MessageEnvelope,byte[]>
	{
		private readonly MemoryPool m_Pool=new AlwaysAllocatePool();

		public int HeaderSize
		{
			get{return MessageEnvelope.EnvelopeSize;}
		}

		public MessageEnvelope CreateHeader(byte[] buffer)
		{
			using(var stream=new MemoryStream(buffer))
			using(var decoder=new DataDecoder(stream))
			{
				var envelope=new MessageEnvelope(decoder);
				return envelope;
			}
		}

		public byte[] CreateBody(MessageEnvelope header, byte[] buffer, int bodySize)
		{
			throw new NotImplementedException();
		}

		public int GetBodySize(MessageEnvelope header)
		{
			return header.DataLength;
		}

		public Memory.Pools.MemoryPool GetBodyPool()
		{
			return m_Pool;
		}
	}
}
