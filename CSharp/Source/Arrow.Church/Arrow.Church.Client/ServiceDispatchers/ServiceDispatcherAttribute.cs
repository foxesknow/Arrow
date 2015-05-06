using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Factory;

namespace Arrow.Church.Client.ServiceDispatchers
{
	public sealed class ServiceDispatcherAttribute : RegisteredTypeAttribute
	{
		public ServiceDispatcherAttribute(string name) : base(typeof(ServiceDispatcherFactory),name)
		{
		}
	}
}
