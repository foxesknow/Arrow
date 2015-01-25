using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Messaging.Support;

namespace Arrow.Messaging.Memory.Messages
{
	[Serializable]
	class MemByteMessage : ByteMessage, IMemoryMessage
	{
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="data">The byte data that makes up the message</param>
		public MemByteMessage(byte[] data) : base(data)
		{
		}
	}
}
