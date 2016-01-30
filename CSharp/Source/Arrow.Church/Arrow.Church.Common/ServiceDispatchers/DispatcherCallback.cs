using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Church.Common.Net;
using Arrow.Collections;

namespace Arrow.Church.Common.ServiceDispatchers
{
	public delegate void DispatcherCallback(MessageEnvelope messageEnvelope, ArraySegmentCollection<byte> buffers);
}
