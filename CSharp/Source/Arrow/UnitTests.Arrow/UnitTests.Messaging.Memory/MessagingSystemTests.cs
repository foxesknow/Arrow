using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Messaging;
using Arrow.Messaging.Memory;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace UnitTests.Arrow.Messaging.Memory
{
	[TestFixture]
	public class MessagingSystemTests
	{
		[Test]
		public void CreateSender()
		{
			using(var topic=MessagingSystem.CreateSender(MemoryScheme.MemoryTopic))
			{
				Assert.That(topic,Is.Not.Null);
				Assert.That(topic,Is.TypeOf<MemoryMessageSender>());
				Assert.That(topic.IsConnected,Is.False);
			}

			using(var queue=MessagingSystem.CreateSender(MemoryScheme.MemoryQueue))
			{
				Assert.That(queue,Is.Not.Null);
				Assert.That(queue,Is.TypeOf<MemoryMessageSender>());
				Assert.That(queue.IsConnected,Is.False);
			}
		}

		[Test]
		public void CreateReceiver()
		{
			using(var topic=MessagingSystem.CreateReceiver(MemoryScheme.MemoryTopic))
			{
				Assert.That(topic,Is.Not.Null);
				Assert.That(topic,Is.TypeOf<MemoryMessageReceiver>());
				Assert.That(topic.IsConnected,Is.False);
			}

			using(var queue=MessagingSystem.CreateReceiver(MemoryScheme.MemoryQueue))
			{
				Assert.That(queue,Is.Not.Null);
				Assert.That(queue,Is.TypeOf<MemoryMessageReceiver>());
				Assert.That(queue.IsConnected,Is.False);
			}
		}

		[Test]
		public void GetMessagingSystem()
		{
			var topicSystem=MessagingSystem.Create(MemoryScheme.MemoryTopic);
			Assert.That(topicSystem,Is.Not.Null);

			var queueSystem=MessagingSystem.Create(MemoryScheme.MemoryQueue);
			Assert.That(queueSystem,Is.Not.Null);
		}
	}
}
