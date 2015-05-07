using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Server.ServiceListeners
{
	[ServiceListener("inproc")]
	public sealed class InProcessServiceListenerCreator : ServiceListenerCreator
	{
		public override ServiceListener Create(Uri endpoint)
		{
			return new InProcessServiceListener(endpoint);
		}
	}
}
