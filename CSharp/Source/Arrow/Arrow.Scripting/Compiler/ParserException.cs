using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Compiler
{
	/// <summary>
	/// Represents a parse exception
	/// </summary>
	[Serializable]
	public class ParserException : ArrowException
	{
		/// <summary>
		/// Initializes the instance
		/// </summary>
		public ParserException() { }

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="message"></param>
		public ParserException(string message) : base(message) { }
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public ParserException(string message, Exception inner) : base(message, inner) { }
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected ParserException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	
		/// <summary>
		/// The line that the error occurred on
		/// </summary>
		public int LineNumber{get;set;}

		/// <summary>
		/// The name of the file the error occurred in
		/// </summary>
		public string Filename{get;set;}
	}
}
