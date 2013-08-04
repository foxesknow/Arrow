using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;

using Arrow.Scripting;

namespace Arrow.Scripting.Wire
{
	abstract class ExpressionFactory
	{
		public static readonly BindingFlags InstanceFlags=BindingFlags.Instance|BindingFlags.IgnoreCase|BindingFlags.Public;
		public static readonly BindingFlags StaticFlags=BindingFlags.Static|BindingFlags.IgnoreCase|BindingFlags.Public;

		protected readonly Parser m_Parser;

		protected ExpressionFactory(Parser parser)
		{
			m_Parser=parser;
		}

		protected Parser Parser
		{
			get{return m_Parser;}
		}

		public abstract Expression LogicalAnd(Expression lhs, Expression rhs);
		public abstract Expression LogicalOr(Expression lhs, Expression rhs);
		public abstract Expression NullCoalesce(Expression lhs, Expression rhs);

		public abstract Expression Equal(CaseMode caseMode, Expression lhs, Expression rhs);
		public abstract Expression NotEqual(CaseMode caseMode, Expression lhs, Expression rhs);
		public abstract Expression RegexEqual(Expression lhs, Expression rhs);
		public abstract Expression Like(CaseMode caseMode, Expression lhs, Expression pattern);
		
		public abstract Expression GreaterThan(Expression lhs, Expression rhs);
		public abstract Expression GreaterThanOrEqual(Expression lhs, Expression rhs);
		public abstract Expression LessThan(Expression lhs, Expression rhs);
		public abstract Expression LessThanOrEqual(Expression lhs, Expression rhs);

		public abstract Expression Add(Expression lhs, Expression rhs);
		public abstract Expression Subtract(Expression lhs, Expression rhs);

		public abstract Expression Multiply(Expression lhs, Expression rhs);
		public abstract Expression Divide(Expression lhs, Expression rhs);
		public abstract Expression Modulo(Expression lhs, Expression rhs);

		public abstract Expression Not(Expression target);
		public abstract Expression UnaryPlus(Expression target);
		public abstract Expression UnaryMinus(Expression target);

		public abstract Expression ArrayAccess(Expression target, IEnumerable<Expression> bounds);

		public abstract Expression Cast(Expression target, Type type);
		public abstract Expression Is(Expression target, Type type);
		public abstract Expression As(Expression target, Type type);

		public abstract Expression ToBoolean(Expression target);

		public abstract Expression StaticCall(Type type, string name, IEnumerable<Expression> parameters);
		public abstract Expression StaticPropertyOrFieldWithArgs(Type type, string name, IEnumerable<Expression> indexes);
		public abstract Expression StaticPropertyOrField(Type type, string name);

		public abstract Expression InstanceCall(Expression instance, string name, IEnumerable<Expression> parameters);
		public abstract Expression InstancePropertyOrFieldWithArgs(Expression instance, string name, IEnumerable<Expression> indexes);
		public abstract Expression InstancePropertyOrField(Expression instance, string name);

		public abstract Expression Ternary(Expression condition, Expression ifTrue, Expression ifFalse);
	}
}
