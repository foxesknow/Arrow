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
		public PingResponse(long pingID)
		{
			this.PingID=pingID;
			this.ServerLocal=DateTime.Now;
		}

		/// <summary>
		/// The PingID sent by the client
		/// </summary>
		public long PingID{get;private set;}
		
		/// <summary>
		/// The local time on the server
		/// </summary>
		public DateTime ServerLocal{get;private set;}

		public override string ToString()
		{
			return string.Format("ID={0}, ServerLocal={1}",this.PingID,this.ServerLocal);
		}
	}
}
