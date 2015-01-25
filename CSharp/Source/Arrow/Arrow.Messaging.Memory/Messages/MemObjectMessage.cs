using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Messaging.Support;

namespace Arrow.Messaging.Memory.Messages
{
	[Serializable]
	class MemObjectMessage : ObjectMessage, IMemoryMessage
	{
		/// <summary>
		/// Initializes the intstance
		/// </summary>
		/// <param name="theObject">The object the message holds</param>
		public MemObjectMessage(object theObject) : base(theObject)
		{
		}
	}
}
