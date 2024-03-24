using System;
using System.Collections.Generic;
using System.Text;

namespace Arrow.GraphTheory.Build
{
	/// <summary>
	/// Base class for build exceptions
	/// </summary>
	[global::System.Serializable]
	public class BuildException : ArrowException
	{
		/// <summary>
		/// 
		/// </summary>
		public BuildException() { }
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public BuildException(string message) : base(message) { }
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public BuildException(string message, Exception inner) : base(message, inner) { }		
	}
}
