using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Threading;

namespace Arrow.Church.Client.ServiceDispatchers
{
	public sealed class NullServiceDispatcher : ServiceDispatcher
	{
		public NullServiceDispatcher(Uri endpoint) : base(endpoint)
		{
		}

		protected override void SendRequest(Common.Net.MessageEnvelope envelope, byte[] data)
		{
			var correlationID=envelope.MessageCorrelationID;
			CompleteError(correlationID,new NotImplementedException());
		}
	}
}
