using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Messaging
{
	/// <summary>
	/// Base class for all message receivers.
	/// NOTE: MessageReceiver instances are not thread safe. They should only be used by the thread that create them
	/// 
	/// A receiver operates in either synchronous or asynchronous mode, but not both.
	/// To operate synchronously you use the Receive() methods.
	/// To operate asychronously you attach MessageReceived handlers
	/// </summary>
	public abstract class MessageReceiver : MessageClient
	{
		/// <summary>
		/// Raised when a message is received
		/// </summary>
		public abstract event EventHandler<MessageEventArgs> MessageReceived;
	
		/// <summary>
		/// Initializes the instance
		/// </summary>
		protected MessageReceiver()
		{
		}
		
		/// <summary>
		/// A message selection string for messaging systems that support filtering
		/// </summary>
		public virtual string MessageSelector{get;set;}
				
		/// <summary>
		/// Receives a message, blocking until one is available
		/// </summary>
		/// <returns>The message received from the messaging resource</returns>
		public virtual IMessage Receive()
		{
			return Receive(0);
		}
		
		/// <summary>
		/// Receives a message if one is immediately available, otherwise returns null
		/// </summary>
		/// <returns>The message received from the messaging resource, or null if one was not received</returns>
		public virtual IMessage ReceiveNoWait()
		{
			return Receive(-1);
		}
		
		/// <summary>
		/// Returns true is the user can call Receive(), otherwise false
		/// </summary>
		public abstract bool CanReceive{get;}
		
		/// <summary>
		/// Waits a specific amount of time for a message to become available
		/// </summary>
		/// <param name="timespan">How long to wait for a message, in milliseconds</param>
		/// <returns>The message received from the messaging resource, or null if one was not received</returns>
		public virtual IMessage Receive(TimeSpan timespan)
		{
			return Receive((long)timespan.TotalMilliseconds);
		}
		
		/// <summary>
		/// Waits a specific amount of time for a message to become available
		/// </summary>
		/// <param name="milliseconds">How long to wait. 0 means wait forever, -1 means don't wait</param>
		/// <returns>The message received from the messaging resource, or null if one was not received</returns>
		public abstract IMessage Receive(long milliseconds);			
	}
}
