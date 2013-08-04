using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow
{

	/// <summary>
	/// Base class for all Arrow exceptions
	/// </summary>
	[Serializable]
	public class ArrowException : Exception
	{
		/// <summary>
		/// 
		/// </summary>
		public ArrowException() { }
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public ArrowException(string message) : base(message) { }
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public ArrowException(string message, Exception inner) : base(message, inner) { }
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected ArrowException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}
