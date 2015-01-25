using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Factory;

namespace Arrow.Messaging
{
	/// <summary>
	/// Registers a messaging system with the framework.
	/// A messaging system may have multiple names
	/// </summary>
	public class MessagingSystemAttribute : RegisteredTypeAttribute
	{
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="name">The name to expose the messaging system as</param>
		public MessagingSystemAttribute(string name) : base(typeof(MessagingSystemFactory),name)
		{
		}
	}		
}
