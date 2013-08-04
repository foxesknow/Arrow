using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Dynamic
{
	public enum ParameterMatchType
	{
		/// <summary>
		/// The types must match exactly
		/// </summary>
		Exact,

		/// <summary>
		/// The types may be cast to resolve parameters
		/// </summary>
		Cast
	}
}
