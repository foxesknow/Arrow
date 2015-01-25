using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Messaging
{
	/// <summary>
	/// Defines the behaviour of a message containing text
	/// </summary>
	public interface ITextMessage : IMessage
	{
		/// <summary>
		/// The text in the message
		/// </summary>
		string Text{get;}
	}
}
