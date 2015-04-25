using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Church.Common.Net;

namespace Arrow.Church.Server
{
	public abstract class ServiceListener
	{
		/// <summary>
		/// Raised when a service call is received
		/// </summary>
		public event EventHandler<ServiceCallEventArgs> ServiceCall;

		/// <summary>
		/// Sends a response back to the sender
		/// </summary>
		/// <param name="senderMessageEnvelope"></param>
		/// <param name="buffers"></param>
		public abstract void Respond(MessageEnvelope senderMessageEnvelope, IList<ArraySegment<byte>> buffers);

		protected void OnServiceCall(ServiceCallEventArgs args)
		{
			var d=ServiceCall;
			if(d!=null) d(this,args);
		}
	}
}
