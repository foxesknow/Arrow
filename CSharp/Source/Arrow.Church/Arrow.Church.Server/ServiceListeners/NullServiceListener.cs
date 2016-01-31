using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Memory;
using Arrow.Threading;

namespace Arrow.Church.Server.ServiceListeners
{
	public sealed class NullServiceListener : ServiceListener
	{
		public NullServiceListener(Uri endpoint) : base(endpoint)
		{
		}

		public override void Start()
		{
		}

		public override void Stop()
		{
		}

		public override Task RespondAsync(CallDetails callDetails, ArraySegmentCollection<byte> buffers)
		{
			return TaskEx.FromException(new NotImplementedException());
		}
	}
}
