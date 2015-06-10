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
			sizeof(long)+	// MessageSystemID
			sizeof(long)+	// MessageCorrelationID
			sizeof(long)+	// ResponseSystemID
			sizeof(long)+	// ResponseCorrelationID
			sizeof(long)+	// SessionHigh
			sizeof(long)+	// SessionLow
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
			this.MessageSystemID=decoder.ReadInt64();
			this.MessageCorrelationID=decoder.ReadInt64();
			this.ResponseSystemID=decoder.ReadInt64();
			this.ResponseCorrelationID=decoder.ReadInt64();
			this.SessionHigh=decoder.ReadInt64();
			this.SessionLow=decoder.ReadInt64();

			int end=decoder.ReadInt32();
			if(end!=EndMagicNumber) throw new IOException("EndMagicNumber not found");
		}

		void IEncodeData.Encode(DataEncoder encoder)
		{
			encoder.Write(BeginMagicNumber);
			encoder.Write(this.MessageType);
			encoder.Write(this.DataLength);
			encoder.Write(this.MessageSystemID);
			encoder.Write(this.MessageCorrelationID);
			encoder.Write(this.ResponseSystemID);
			encoder.Write(this.ResponseCorrelationID);
			encoder.Write(this.SessionHigh);
			encoder.Write(this.SessionLow);
			encoder.Write(EndMagicNumber);
		}

		public int MessageType{get;set;}
		public int DataLength{get;set;}
		
		/// <summary>
		/// The system ID of the endpoint making the request
		/// </summary>
		public long MessageSystemID{get;set;}
		
		/// <summary>
		/// The system ID of the endpoint making the request
		/// </summary>
		public long MessageCorrelationID{get;set;}

		/// <summary>
		/// In a response message this is the original MessageSystemID
		/// </summary>
		public long ResponseSystemID{get;set;}
		
		/// <summary>
		/// In a response this is the original MessageCorrelationID
		/// </summary>
		public long ResponseCorrelationID{get;set;}

		public long SessionHigh{get;set;}

		public long SessionLow{get;set;}
	}
}
