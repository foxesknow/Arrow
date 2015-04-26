using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow;

namespace Arrow.Church.Common
{
	public class ChurchException : ArrowException
	{
		/// <summary>
		/// 
		/// </summary>
		public ChurchException() { }
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public ChurchException(string message) : base(message) { }
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public ChurchException(string message, Exception inner) : base(message, inner) { }
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected ChurchException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}
