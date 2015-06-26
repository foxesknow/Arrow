using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace Arrow.Church.Protobuf.Common.Services.ProtoPing
{
	[ProtoContract]
	public class ProtoPingResponse
	{
		/// <summary>
		/// The PingID sent by the client
		/// </summary>
		[ProtoMember(1)]
		public long ClientPingID{get;set;}

		/// <summary>
		/// The PingID sent by the server
		/// </summary>
		[ProtoMember(2)]
		public long ServerPingID{get;set;}
		
		public override string ToString()
		{
			return string.Format("ClientPingID={0}, ServerPingID={1}",this.ClientPingID,this.ServerPingID);
		}
	}
}
