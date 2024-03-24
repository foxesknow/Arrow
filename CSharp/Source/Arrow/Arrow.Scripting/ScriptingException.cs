using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Scripting
{
	/// <summary>
	/// Base class for all scripting exceptions
	/// </summary>
	[Serializable]
	public class ScriptingException : ArrowException
	{
		/// <summary>
		/// Initializes the instance
		/// </summary>
		public ScriptingException() { }
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="message"></param>
		public ScriptingException(string message) : base(message) { }
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public ScriptingException(string message, Exception inner) : base(message, inner) { }
	}
}
