using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Messaging
{
	/// <summary>
	/// Implemented by senders who are able to receive data as a binary stream
	/// </summary>
	public interface IByteStreamMessageReceiver
	{
		/// <summary>
		/// Raised when a message has been received. No processing is done on the contents
		/// </summary>
		event EventHandler<ByteStreamMessageEventArgs> ByteStreamMessageReceived;
	}
}
