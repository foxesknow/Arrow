using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Arrow.Collections;
using Arrow.Serialization;

namespace Arrow.Messaging.Memory
{
	/// <summary>
	/// A queue session is shared amongst all instance of a queue
	/// </summary>
	class QueueSession : Session
	{
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="address">The name of the queue</param>
		public QueueSession(Uri address) : base(address)
		{
			this.BufferMessages=true;
		}

		protected override void DispatchMessage()
		{
			// NOTE: A null message tells the receiver to grab a message itself
			Func<MessageEventArgs> argFactory=()=>
			{
				return null;
			};

			OnMessageAvailable(argFactory);
		}
	}
}
