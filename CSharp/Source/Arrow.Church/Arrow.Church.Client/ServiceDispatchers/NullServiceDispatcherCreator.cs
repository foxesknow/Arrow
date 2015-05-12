using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Client.ServiceDispatchers
{
	[ServiceDispatcher("null")]
	public sealed class NullServiceDispatcherCreator : ServiceDispatcherCreator
	{
		public override ServiceDispatcher Create(Uri endpoint)
		{
			return new NullServiceDispatcher(endpoint);
		}
	}
}
