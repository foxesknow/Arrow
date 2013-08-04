using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow;

namespace Arrow.Dynamic
{

	[Serializable]
	public class DynamicException : ArrowException
	{
		/// <summary>
		/// Initializes the instance
		/// </summary>
		public DynamicException() { }
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="message"></param>
		public DynamicException(string message) : base(message) { }
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public DynamicException(string message, Exception inner) : base(message, inner) { }
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected DynamicException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}
