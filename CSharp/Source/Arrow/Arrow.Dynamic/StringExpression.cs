using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;

using Arrow.Scripting;
using Arrow.Dynamic;

namespace Arrow.Dynamic
{
	public static class StringExpression
	{
		private static readonly MethodInfo CompareMethod=typeof(string).GetMethod("Compare",TypeArray.Make<string,string,bool>())!;
		private static readonly MethodInfo ConcatMethod=typeof(string).GetMethod("Concat",TypeArray.Make<string,string>())!;

		public static Expression Equal(CaseMode caseMode, Expression lhs, Expression rhs)
		{
			CheckSides(lhs,rhs);

			Expression? expression=null;

			if(caseMode==CaseMode.Sensitive)
			{
				expression=Expression.Equal(lhs,rhs);
			}
			else
			{
				expression=GenerateFromCompare(caseMode,Expression.Equal,lhs,rhs);
			}

			return expression;
		}

		public static Expression NotEqual(CaseMode caseMode, Expression lhs, Expression rhs)
		{
			CheckSides(lhs,rhs);

			Expression? expression=null;

			if(caseMode==CaseMode.Sensitive)
			{
				expression=Expression.NotEqual(lhs,rhs);
			}
			else
			{
				expression=GenerateFromCompare(caseMode,Expression.NotEqual,lhs,rhs);
			}

			return expression;
		}

		public static Expression GreaterThan(CaseMode caseMode, Expression lhs, Expression rhs)
		{
			CheckSides(lhs,rhs);
			return GenerateFromCompare(caseMode,Expression.GreaterThan,lhs,rhs);
		}

		public static Expression GreaterThanOrEqual(CaseMode caseMode, Expression lhs, Expression rhs)
		{
			CheckSides(lhs,rhs);
			return GenerateFromCompare(caseMode,Expression.GreaterThanOrEqual,lhs,rhs);
		}

		public static Expression LessThan(CaseMode caseMode, Expression lhs, Expression rhs)
		{
			CheckSides(lhs,rhs);
			return GenerateFromCompare(caseMode,Expression.LessThan,lhs,rhs);
		}

		public static Expression LessThanOrEqual(CaseMode caseMode, Expression lhs, Expression rhs)
		{
			CheckSides(lhs,rhs);
			return GenerateFromCompare(caseMode,Expression.LessThanOrEqual,lhs,rhs);
		}

		public static Expression Concat(Expression lhs, Expression rhs)
		{
			CheckSides(lhs,rhs);
			return Expression.Call(ConcatMethod,lhs,rhs);
		}

		private static Expression GenerateFromCompare(CaseMode caseMode, Func<Expression,Expression,Expression> comparer, Expression lhs, Expression rhs)
		{
			Expression ignoreCase=(caseMode==CaseMode.Insensitive ? ExpressionConstants.True : ExpressionConstants.False);

			Expression call=Expression.Call(CompareMethod,lhs,rhs,ignoreCase);			
			Expression comparison=comparer(call,ExpressionConstants.IntegerZero);

			return comparison;
		}

		private static void CheckSides(Expression lhs, Expression rhs)
		{
			if(lhs==null) throw new ArgumentNullException("lhs");
			if(rhs==null) throw new ArgumentNullException("rhs");

			if(lhs.Type!=typeof(string)) throw new DynamicException("lhs not a string");
			if(rhs.Type!=typeof(string)) throw new DynamicException("rhs not a string");
		}
	}
}
