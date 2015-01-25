using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Messaging
{
	/// <summary>
	/// Implemented by senders that support sending data as a raw byte stream
	/// </summary>
	public interface IByteStreamMessageSender
	{
		/// <summary>
		/// Sends a byte stream to messaging system
		/// </summary>
		/// <param name="byteStream">The byte data to send</param>
		void Send(byte[] byteStream);
	}
}
