using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Arrow.Collections;
using Arrow.Serialization;

namespace Arrow.Messaging.Memory
{
	class TopicSession : Session
	{
        public TopicSession(Uri address) : base(address)
        {
            this.BufferMessages = false;
        }

        public override void Send(IMessage message)
        {
            // Only send if there are actually subscribers waiting
            if(this.MessageAvailableCount != 0)
            {
                base.Send(message);
            }
        }

        protected override void DispatchMessage()
        {
            byte[] messageData;

            // We create a new MessageEventArgs per subscriber to
            // isolate multiple subscribers from any changes made
            // to the underlying message by a subscriber
            if(TryGetBytes(out messageData))
            {
                Func<MessageEventArgs> argFactory = () =>
                {
                    var message = GenericBinaryFormatter.FromArray<IMessage>(messageData);
                    return new MessageEventArgs(message);
                };

                OnMessageAvailable(argFactory);
            }
        }
    }
}
