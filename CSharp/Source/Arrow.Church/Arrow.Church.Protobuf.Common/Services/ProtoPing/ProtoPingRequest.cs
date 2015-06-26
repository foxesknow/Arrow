using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ProtoBuf;

namespace Arrow.Church.Protobuf.Common.Services.ProtoPing
{
	[ProtoContract]
	public class ProtoPingRequest
	{
		private static long s_PingID;

		public ProtoPingRequest()
		{
			this.PingID=Interlocked.Increment(ref s_PingID);
			this.ClientLocal=DateTime.Now;
		}

		/// <summary>
		/// An ID that will be sent to the server.
		/// This ID will come back in the server response
		/// </summary>
		[ProtoMember(1)]
		public long PingID{get;set;}
		
		/// <summary>
		/// The local time on the sender
		/// </summary>
		[ProtoMember(2)]
		public DateTime ClientLocal{get;set;}

		public override string ToString()
		{
			return string.Format("ID={0}, ClientLocal={1}",this.PingID,this.ClientLocal);
		}
	}
}
