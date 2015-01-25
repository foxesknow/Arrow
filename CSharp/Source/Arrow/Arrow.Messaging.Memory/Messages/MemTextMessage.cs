using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Messaging.Support;

namespace Arrow.Messaging.Memory.Messages
{
	[Serializable]
	class MemTextMessage : TextMessage, IMemoryMessage
	{
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="text">The text message that makes up the instance</param>
		public MemTextMessage(string text) : base(text)
		{
		}
	}
}
