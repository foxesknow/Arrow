﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Church.Common;

namespace Arrow.Church.Common
{
	[Serializable]
	public class ServiceNotFoundException : ChurchException
	{
		/// <summary>
		/// 
		/// </summary>
		public ServiceNotFoundException() { }
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public ServiceNotFoundException(string message) : base(message) { }
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public ServiceNotFoundException(string message, Exception inner) : base(message, inner) { }
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected ServiceNotFoundException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}
