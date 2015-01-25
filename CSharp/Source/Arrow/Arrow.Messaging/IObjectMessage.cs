using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Messaging
{
	/// <summary>
	/// Defines the behaviour of a message containing an object
	/// </summary>
	public interface IObjectMessage : IMessage
	{
		/// <summary>
		/// The object held by the message
		/// </summary>
		object TheObject{get;}
	}
}
