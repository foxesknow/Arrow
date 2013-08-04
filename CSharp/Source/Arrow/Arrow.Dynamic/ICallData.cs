using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Dynamic
{
	/// <summary>
	/// Provides information on a call
	/// </summary>
	public interface ICallData
	{
		/// <summary>
		/// The cost of the call
		/// </summary>
		int Cost{get;}
	}
}
