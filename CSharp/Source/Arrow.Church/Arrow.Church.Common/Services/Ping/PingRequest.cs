using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.Church.Common.Services.Ping
{
	[Serializable]
	public sealed class PingRequest
	{
		private static long s_PingID;

		public PingRequest()
		{
			this.PingID=Interlocked.Increment(ref s_PingID);
			this.ClientLocal=DateTime.Now;
		}

		/// <summary>
		/// An ID that will be sent to the server.
		/// This ID will come back in the server response
		/// </summary>
		public long PingID{get;private set;}
		
		/// <summary>
		/// The local time on the sender
		/// </summary>
		public DateTime ClientLocal{get;private set;}

		public override string ToString()
		{
			return string.Format("ID={0}, ClientLocal={1}",this.PingID,this.ClientLocal);
		}
	}
}
