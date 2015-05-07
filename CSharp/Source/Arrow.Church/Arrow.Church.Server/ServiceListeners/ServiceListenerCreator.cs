using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Server.ServiceListeners
{
	public abstract class ServiceListenerCreator
	{
		public abstract ServiceListener Create(Uri endpoint);
	}
}
