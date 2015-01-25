using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Messaging
{
	/// <summary>
	/// Base class for messaging exceptions
	/// </summary>
	[global::System.Serializable]
	public class MessagingException : ArrowException
	{
		/// <summary>
		/// 
		/// </summary>
		public MessagingException() { }
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public MessagingException(string message) : base(message) { }
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public MessagingException(string message,Exception inner) : base(message,inner) { }
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected MessagingException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info,context) { }
	}
}
