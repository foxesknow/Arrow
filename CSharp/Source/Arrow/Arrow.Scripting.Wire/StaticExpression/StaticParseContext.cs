using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Arrow.Scripting.Wire.StaticExpression
{
	public class StaticParseContext : ParseContext
	{
		private readonly List<ParameterExpression> m_Parameters=new List<ParameterExpression>();

		/// <summary>
		/// The parameters to pass to the lambda
		/// </summary>
		public IList<ParameterExpression> Parameters
		{
			get{return m_Parameters;}
		}
	}
}
