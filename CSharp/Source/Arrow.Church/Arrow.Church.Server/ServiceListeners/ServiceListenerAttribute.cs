using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Factory;

namespace Arrow.Church.Server.ServiceListeners
{
	class ServiceListenerAttribute : RegisteredTypeAttribute
	{
		public ServiceListenerAttribute(string name) : base(typeof(ServiceListenerFactory),name)
		{
		}
	}
}
