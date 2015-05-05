using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Common.Internal
{
	public static class NetworkExtensions
	{
		public static IPAddress TryResolveIPAddress(this Uri endpoint)
		{
			string host=endpoint.Host;

			IPAddress address=null;
			if(IPAddress.TryParse(host,out address)) return address;

			var hostAddresses=Dns.GetHostAddresses(host);
			foreach(var hostAddress in hostAddresses)
			{
				if(hostAddress.AddressFamily==AddressFamily.InterNetwork)
				{
					return hostAddress;
				}
			}

			return null;
		}
	}
}
