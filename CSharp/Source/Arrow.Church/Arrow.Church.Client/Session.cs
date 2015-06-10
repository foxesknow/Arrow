using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Client
{
	/// <summary>
	/// Manages client-side session information.
	/// 
	/// It's main purpose is to create a session identifier that is regenerated every time the client side application starts.
	/// This identifer is useful in pub-sub style architectures where the client may shut down before it has received all the 
	/// responses to its requests. By using the session-id in the pub-sub subscription (eg as a filter) the client can ensure
	/// it doesn't accidentally receive any messages from the previous run
	/// </summary>
	public static class Session
	{
		public static readonly Guid ID;
		public static readonly string IDString;
		public static readonly long SessionHigh;
		public static readonly long SessionLow;

		static Session()
		{
			ID=Guid.NewGuid();
			IDString=ID.ToString("N");

			byte [] data=ID.ToByteArray();

			SessionHigh=BitConverter.ToInt64(data,8);
			SessionLow=BitConverter.ToInt64(data,0);
		}
	}
}
