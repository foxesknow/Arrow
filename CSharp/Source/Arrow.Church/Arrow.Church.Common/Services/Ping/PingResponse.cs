using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Common.Services.Ping
{
	[Serializable]
	public sealed class PingResponse
	{
		public PingResponse(long clientPingID, long serverPingID)
		{
			this.ClientPingID=clientPingID;
			this.ServerPingID=serverPingID;
			this.ServerLocal=DateTime.Now;
		}

		/// <summary>
		/// The PingID sent by the client
		/// </summary>
		public long ClientPingID{get;private set;}

		/// <summary>
		/// The PingID sent by the server
		/// </summary>
		public long ServerPingID{get;private set;}
		
		/// <summary>
		/// The local time on the server
		/// </summary>
		public DateTime ServerLocal{get;private set;}

		public override string ToString()
		{
			return string.Format("ClientPingID={0}, ServerPingID={1}, ServerLocal={2}",this.ClientPingID,this.ServerPingID,this.ServerLocal);
		}
	}
}
