using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Server.ServiceListeners
{
	[ServiceListener("net")]
	public sealed class NetworkServiceListenerCreator : ServiceListenerCreator
	{
		public override ServiceListener Create(Uri endpoint)
		{
			return new NetworkServiceListener(endpoint);
		}
	}
}
