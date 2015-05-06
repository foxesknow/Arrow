using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Client.ServiceDispatchers
{
	public abstract class ServiceDispatcherCreator
	{
		public abstract ServiceDispatcher Create(Uri endpoint);
	}
}
