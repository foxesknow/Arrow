using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Church.Common.Data;
using Arrow.Church.Common.Net;
using Arrow.Church.Common.ServiceDispatchers;
using Arrow.Threading;

namespace Arrow.Church.Client.ServiceDispatchers
{
	public class InProcessServiceDispatcher : ServiceDispatcher
	{
		public InProcessServiceDispatcher(Uri endpoint) : base(endpoint)
		{

		}

		protected override Task SendRequestAsync(MessageEnvelope envelope, byte[] data)
		{
			InProcessServiceDispatcherRouter.Enqueue(this.Endpoint,envelope,data,HandleResponse);
			return Task.FromResult(true);
		}
	}
}
