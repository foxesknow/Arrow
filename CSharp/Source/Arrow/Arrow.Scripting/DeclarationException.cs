using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Scripting
{	
	/// <summary>
	/// Throws when a variable could not be declared
	/// </summary>
	[Serializable]
	public class DeclarationException : ScriptingException
	{
		/// <summary>
		/// Initializes the instance
		/// </summary>
		public DeclarationException() { }
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="message"></param>
		public DeclarationException(string message) : base(message) { }
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public DeclarationException(string message, Exception inner) : base(message, inner) { }
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected DeclarationException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}
