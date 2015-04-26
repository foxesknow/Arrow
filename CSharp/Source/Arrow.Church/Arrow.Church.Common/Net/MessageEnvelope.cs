using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Church.Common.Data;

namespace Arrow.Church.Common.Net
{
	public class MessageEnvelope : IEncodeData
	{
		private const int BeginMagicNumber=0x59415250; // 'PRAY
		private const int EndMagicNumber=0x79617270; // 'pray

		public const int EnvelopeSize=
		(
			sizeof(int)+	// BeginMagicNumber
			sizeof(int)+	// MessageType
			sizeof(int)+	// DataLength
			sizeof(long)+	// SenderSystemID
			sizeof(long)+	// SenderCorrelationID
			sizeof(int)		// EndMagicNumber
		);

		public MessageEnvelope()
		{
		}

		public MessageEnvelope(DataDecoder decoder)
		{
			if(decoder==null) throw new ArgumentNullException("decoder");

			int begin=decoder.ReadInt32();
			if(begin!=BeginMagicNumber) throw new IOException("BeginMagicNumber not found");

			this.MessageType=decoder.ReadInt32();
			this.DataLength=decoder.ReadInt32();
			this.SenderSystemID=decoder.ReadInt64();
			this.SenderCorrelationID=decoder.ReadInt64();

			int end=decoder.ReadInt32();
			if(end!=EndMagicNumber) throw new IOException("EndMagicNumber not found");
		}

		void IEncodeData.Encode(DataEncoder encoder)
		{
			encoder.Write(BeginMagicNumber);
			encoder.Write(this.MessageType);
			encoder.Write(this.DataLength);
			encoder.Write(this.SenderSystemID);
			encoder.Write(this.SenderCorrelationID);
			encoder.Write(EndMagicNumber);
		}

		public int MessageType{get;set;}
		public int DataLength{get;set;}
		public long SenderSystemID{get;set;}
		public long SenderCorrelationID{get;set;}
	}
}
