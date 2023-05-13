using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Arrow.Messaging;
using Arrow.Messaging.Memory;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace UnitTests.Arrow.Messaging.Memory
{
    [TestFixture]
    public class MessagingTests
    {
        private static readonly Uri TopicAddress = new Uri("memtopic:///foo");
        private static readonly Uri QueueAddress = new Uri("memqueue:///bar");

        [Test]
        public void Construction()
        {
            using(var sender = MessagingSystem.CreateSender(TopicAddress))
            {
                Assert.That(sender, Is.TypeOf<MemoryMessageSender>());
            }
        }

        [Test]
        public void CreateMessages()
        {
            using(var sender = MessagingSystem.CreateSender(TopicAddress))
            {
                sender.Connect(TopicAddress);

                var textMessage = sender.CreateTextMessage("Jack");
                Assert.That(textMessage, Is.Not.Null);
                Assert.That(textMessage.Text, Is.EqualTo("Jack"));
                EnsureSerializable(textMessage);

                var objectMessage = sender.CreateObjectMessage(1);
                Assert.That(objectMessage, Is.Not.Null);
                Assert.That(objectMessage.TheObject, Is.EqualTo(1));
                EnsureSerializable(objectMessage);

                var byteMessage = sender.CreateByteMessage(new byte[] { 1, 2, 3 });
                Assert.That(byteMessage, Is.Not.Null);
                Assert.That(byteMessage.Data, Is.Not.Null);
                EnsureSerializable(byteMessage);

                var mapMessage = sender.CreateMapMessage();
                Assert.That(mapMessage, Is.Not.Null);
                EnsureSerializable(mapMessage);
            }
        }

        [Test]
        public void SendToQueue()
        {
            using(var gotMessage = new ManualResetEvent(false))
            using(var sender = MessagingSystem.CreateSender(QueueAddress))
            using(var receiver = MessagingSystem.CreateReceiver(QueueAddress))
            {
                bool sent = false;

                sender.Connect(QueueAddress);
                receiver.Connect(QueueAddress);

                receiver.MessageReceived += (s, e) =>
                {
                    sent = true;
                    gotMessage.Set();
                };

                Assert.That(sent, Is.False);

                var message = sender.CreateTextMessage("The Island");
                sender.Send(message);

                gotMessage.WaitOne();
                Assert.That(sent, Is.True);
            }
        }

        [Test]
        public void SendMultipleToQueue()
        {
            int numberToSend = 10;

            using(var gotMessages = new ManualResetEvent(false))
            using(var sender = MessagingSystem.CreateSender(QueueAddress))
            using(var receiver = MessagingSystem.CreateReceiver(QueueAddress))
            {
                bool sent = false;

                sender.Connect(QueueAddress);
                receiver.Connect(QueueAddress);

                int numberReceived = 0;

                receiver.MessageReceived += (s, e) =>
                {
                    if(Interlocked.Increment(ref numberReceived) == numberToSend)
                    {
                        sent = true;
                        gotMessages.Set();
                    }
                };

                Assert.That(sent, Is.False);

                for(int i = 0; i < numberToSend; i++)
                {
                    var message = sender.CreateTextMessage("The Island: " + i.ToString());
                    sender.Send(message);
                }

                gotMessages.WaitOne();
                Assert.That(sent, Is.True);
            }
        }

        [Test]
        public void SendMultipleToMultipleQueue()
        {
            int numberToSend = 10;

            using(var gotMessages = new ManualResetEvent(false))
            using(var sender = MessagingSystem.CreateSender(QueueAddress))
            using(var receiver1 = MessagingSystem.CreateReceiver(QueueAddress))
            using(var receiver2 = MessagingSystem.CreateReceiver(QueueAddress))
            {
                sender.Connect(QueueAddress);
                receiver1.Connect(QueueAddress);
                receiver2.Connect(QueueAddress);

                int numberReceived = 0;

                Action onMessage = () =>
                {
                    if(Interlocked.Increment(ref numberReceived) == numberToSend)
                    {
                        gotMessages.Set();
                    }
                };

                receiver1.MessageReceived += (s, e) => onMessage();
                receiver2.MessageReceived += (s, e) => onMessage();

                for(int i = 0; i < numberToSend; i++)
                {
                    var message = sender.CreateTextMessage("The Island: " + i.ToString());
                    sender.Send(message);
                }

                gotMessages.WaitOne();
                Assert.That(numberReceived, Is.EqualTo(numberToSend));
            }
        }

        [Test]
        public void SendToTopic()
        {
            using(var gotMessage = new ManualResetEvent(false))
            using(var sender = MessagingSystem.CreateSender(TopicAddress))
            using(var receiver = MessagingSystem.CreateReceiver(TopicAddress))
            {
                bool sent = false;

                sender.Connect(TopicAddress);
                receiver.Connect(TopicAddress);

                receiver.MessageReceived += (s, e) =>
                {
                    sent = true;
                    gotMessage.Set();
                };

                Assert.That(sent, Is.False);

                var message = sender.CreateTextMessage("The Island");
                sender.Send(message);

                gotMessage.WaitOne();
                Assert.That(sent, Is.True);
            }
        }

        [Test]
        public void SendMultipleToTopic()
        {
            int numberToSend = 10;

            using(var gotMessages = new ManualResetEvent(false))
            using(var sender = MessagingSystem.CreateSender(TopicAddress))
            using(var receiver = MessagingSystem.CreateReceiver(TopicAddress))
            {
                bool sent = false;

                sender.Connect(TopicAddress);
                receiver.Connect(TopicAddress);

                int numberReceived = 0;

                receiver.MessageReceived += (s, e) =>
                {
                    if(Interlocked.Increment(ref numberReceived) == numberToSend)
                    {
                        sent = true;
                        gotMessages.Set();
                    }
                };

                Assert.That(sent, Is.False);

                for(int i = 0; i < numberToSend; i++)
                {
                    var message = sender.CreateTextMessage("The Island: " + i.ToString());
                    sender.Send(message);
                }

                gotMessages.WaitOne();
                Assert.That(sent, Is.True);
            }
        }

        [Test]
        public void SendMultipleToMultipleTopic()
        {
            int numberToSend = 10;

            using(var gotMessages = new ManualResetEvent(false))
            using(var sender = MessagingSystem.CreateSender(TopicAddress))
            using(var receiver1 = MessagingSystem.CreateReceiver(TopicAddress))
            using(var receiver2 = MessagingSystem.CreateReceiver(TopicAddress))
            {
                sender.Connect(TopicAddress);
                receiver1.Connect(TopicAddress);
                receiver2.Connect(TopicAddress);

                int numberReceived = 0;
                int instances = 2;

                Action onMessage = () =>
                {
                    if(Interlocked.Increment(ref numberReceived) == numberToSend * instances)
                    {
                        gotMessages.Set();
                    }
                };

                receiver1.MessageReceived += (s, e) => onMessage();
                receiver2.MessageReceived += (s, e) => onMessage();

                for(int i = 0; i < numberToSend; i++)
                {
                    var message = sender.CreateTextMessage("The Island: " + i.ToString());
                    sender.Send(message);
                }

                gotMessages.WaitOne();
                Assert.That(numberReceived, Is.EqualTo(numberToSend * instances));
            }
        }

        private void EnsureSerializable(IMessage message)
        {
            Assert.That(message.GetType().IsSerializable, Is.True);
        }
    }
}
