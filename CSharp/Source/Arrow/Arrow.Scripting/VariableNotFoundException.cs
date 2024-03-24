using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Scripting
{	
	/// <summary>
	/// Thrown when a variable could not be found
	/// </summary>
	[Serializable]
	public class VariableNotFoundException : ScriptingException
	{
		/// <summary>
		/// Initializes the instance
		/// </summary>
		public VariableNotFoundException() { }
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="message"></param>
		public VariableNotFoundException(string message) : base(message) { }
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public VariableNotFoundException(string message, Exception inner) : base(message, inner) { }		
	}
}
