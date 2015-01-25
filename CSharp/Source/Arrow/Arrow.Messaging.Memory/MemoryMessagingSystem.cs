using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Messaging.Memory
{
	[MessagingSystem("mem")]
	[MessagingSystem(MemoryScheme.MemoryQueue)]
	[MessagingSystem(MemoryScheme.MemoryTopic)]
	public class MemoryMessagingSystem : MessagingSystem
	{
		public override MessageSender CreateSender()
		{
			return new MemoryMessageSender();
		}

		public override MessageReceiver CreateReceiver()
		{
			return new MemoryMessageReceiver();
		}
	}
}
