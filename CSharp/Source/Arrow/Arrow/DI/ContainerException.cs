using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.DI
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class ContainerException : ArrowException
	{
		/// <summary>
		/// 
		/// </summary>
		public ContainerException() { }
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public ContainerException(string message) : base(message) { }
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public ContainerException(string message, Exception inner) : base(message, inner) { }
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected ContainerException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}
