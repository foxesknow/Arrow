using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Arrow.Dynamic
{
	public static class ExpressionConstants
	{
		/// <summary>
		/// A constant representing true
		/// </summary>
		public static readonly ConstantExpression True=Expression.Constant(true);
		
		/// <summary>
		/// A constant representing false
		/// </summary>
		public static readonly ConstantExpression False=Expression.Constant(false);

		/// <summary>
		/// Integer zero
		/// </summary>
		public static readonly ConstantExpression IntegerZero=Expression.Constant(0);
	}
}
