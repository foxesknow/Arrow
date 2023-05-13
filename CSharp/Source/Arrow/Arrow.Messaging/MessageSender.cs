using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Messaging
{
	/// <summary>
	/// Base class for all message senders
	/// NOTE: MessageSender instances are not thread safe. They should only be used by the thread that create them
	/// </summary>
	public abstract class MessageSender : MessageClient
	{
		/// <summary>
		/// Initializes the instance
		/// </summary>
		protected MessageSender()
		{
		}
		
		/// <summary>
		/// The priority for the message
		/// </summary>
		public int Priority{get; set;}
		
		/// <summary>
		/// How long messages should live in the subsystem (in milliseconds)
		/// zero is unlimited
		/// </summary>
		public long TimeToLive{get; set;}
			
		/// <summary>
		/// Sends a message into the messaging system.
		/// Only messages creates by the instance can be sent
		/// </summary>
		/// <param name="message">The message to send</param>
		public abstract void Send(IMessage message);
				
		/// <summary>
		/// Creates a text message that can be sent via this instance.
		/// This method can only be called after you have connected
		/// </summary>
		/// <param name="text">The text for the message</param>
		/// <returns>A text message</returns>
		public abstract ITextMessage CreateTextMessage(string text);
		
		/// <summary>
		/// Creates a byte message that can be sent via this instance
		/// This method can only be called after you have connected
		/// </summary>
		/// <param name="data">The data for the message</param>
		/// <returns>A byte message</returns>
		public abstract IByteMessage CreateByteMessage(byte[] data);
		
		/// <summary>
		/// Creates a map message that can be sent via this instance
		/// This method can only be called after you have connected
		/// </summary>
		/// <returns>A map message</returns>
		public abstract IMapMessage CreateMapMessage();

		/// <summary>
		/// Creates an object message
		/// </summary>
		/// <param name="theObject">The object for the message</param>
		/// <returns>An object message</returns>
		public abstract IObjectMessage CreateObjectMessage(object theObject);
	}
}
