using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Messaging
{
	/// <summary>
	/// Base event for all message related events
	/// </summary>
	public class MessageEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="message">The message for the event</param>
		public MessageEventArgs(IMessage message)
		{
            if(message == null) throw new ArgumentNullException("message");
            this.Message = message;
        }
	
		/// <summary>
		/// The message the event relates to
		/// </summary>
		public IMessage Message{get; private set;}
	}
}
