using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Arrow.Scripting.Wire.DynamicExpression
{
	public class DynamicParseContext : ParseContext
	{
		/// <summary>
		/// The names of things that will always be treated as a symbol.
		/// </summary>
		public HashSet<string> AlwaysTreatAsSymbol{get;} = new(StringComparer.OrdinalIgnoreCase);
	}
}
