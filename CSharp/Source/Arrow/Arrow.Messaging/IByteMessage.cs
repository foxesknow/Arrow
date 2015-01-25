using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Messaging
{
	/// <summary>
	/// A byte message contains an array of bytes
	/// </summary>
	public interface IByteMessage : IMessage
	{
		/// <summary>
		/// The bytes in the message
		/// </summary>
		byte[] Data{get;}
	}
}
