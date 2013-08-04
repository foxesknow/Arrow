using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Runtime.CompilerServices;

using Arrow.Scripting;
using Arrow.Dynamic;
using Arrow.Reflection;

using Arrow.Scripting.Wire.DynamicExpression.Binders;

namespace Arrow.Scripting.Wire.DynamicExpression
{
	class DynamicExpressionFactory : ExpressionFactory
	{
		public DynamicExpressionFactory(Parser parser) : base(parser)
		{
		}

		public override Expression LogicalAnd(Expression lhs, Expression rhs)
		{
			if(lhs.IsOfType<bool>()==false) lhs=Binder.ToBinary(lhs);
			if(rhs.IsOfType<bool>()==false) rhs=Binder.ToBinary(rhs);

			return Expression.AndAlso(lhs,rhs);			
		}

		public override Expression LogicalOr(Expression lhs, Expression rhs)
		{
			if(lhs.IsOfType<bool>()==false) lhs=Binder.ToBinary(lhs);
			if(rhs.IsOfType<bool>()==false) rhs=Binder.ToBinary(rhs);

			return Expression.OrElse(lhs,rhs);
		}

		public override Expression NullCoalesce(Expression lhs, Expression rhs)
		{
			lhs=lhs.ConvertTo<object>();
			rhs=rhs.ConvertTo<object>();

			return Expression.Coalesce(lhs,rhs);
		}

		public override Expression Equal(CaseMode caseMode, Expression lhs, Expression rhs)
		{
			return Binder.Equality(ExpressionType.Equal,caseMode,lhs,rhs);
		}

		public override Expression RegexEqual(Expression lhs, Expression rhs)
		{
			return Binder.RegexEquality(lhs,rhs);
		}

		public override Expression NotEqual(CaseMode caseMode, Expression lhs, Expression rhs)
		{
			return Binder.Equality(ExpressionType.NotEqual,caseMode,lhs,rhs);
		}

		public override Expression GreaterThan(Expression lhs, Expression rhs)
		{
			return Binder.Relational(Expression.GreaterThan,lhs,rhs);
		}

		public override Expression GreaterThanOrEqual(Expression lhs, Expression rhs)
		{
			return Binder.Relational(Expression.GreaterThanOrEqual,lhs,rhs);
		}

		public override Expression LessThan(Expression lhs, Expression rhs)
		{
			return Binder.Relational(Expression.LessThan,lhs,rhs);
		}

		public override Expression LessThanOrEqual(Expression lhs, Expression rhs)
		{
			return Binder.Relational(Expression.LessThanOrEqual,lhs,rhs);
		}

		public override Expression Add(Expression lhs, Expression rhs)
		{
			return Binder.Binary(ExpressionType.Add,lhs,rhs);
		}

		public override Expression Subtract(Expression lhs, Expression rhs)
		{
			return Binder.Binary(ExpressionType.Subtract,lhs,rhs);
		}

		public override Expression Multiply(Expression lhs, Expression rhs)
		{
			return Binder.Binary(ExpressionType.Multiply,lhs,rhs);
		}

		public override Expression Divide(Expression lhs, Expression rhs)
		{
			return Binder.Binary(ExpressionType.Divide,lhs,rhs);
		}

		public override Expression Modulo(Expression lhs, Expression rhs)
		{
			return Binder.Binary(ExpressionType.Modulo,lhs,rhs);
		}

		public override Expression Not(Expression target)
		{
			var expression=ToBoolean(target);
			return Expression.Not(expression);
		}

		public override Expression UnaryPlus(Expression target)
		{
			return target;
		}

		public override Expression UnaryMinus(Expression target)
		{
			return Binder.Unary(ExpressionType.Negate,target);
		}

		public override Expression ArrayAccess(Expression target, IEnumerable<Expression> indexes)
		{
			return Binder.ArrayAccess(target,indexes);
		}

		public override Expression Cast(Expression target, Type type)
		{
			return Binder.Convert(ExpressionType.Convert,target,type);
		}

		public override Expression Is(Expression target, Type type)
		{
			return Binder.Convert(ExpressionType.TypeIs,target,type);
		}

		public override Expression As(Expression target, Type type)
		{
			return Binder.Convert(ExpressionType.TypeAs,target,type);
		}

		public override Expression ToBoolean(Expression target)
		{
			if(target.IsOfType<bool>()==false) target=Binder.ToBinary(target);
			return target;
		}

		public override Expression StaticCall(Type type, string name, IEnumerable<Expression> parameters)
		{
			return Binder.StaticCall(type,name,parameters);
		}

		public override Expression StaticPropertyOrFieldWithArgs(Type type, string name, IEnumerable<Expression> indexes)
		{
			return Binder.StaticPropertyOrField(type,name,indexes);
		}

		public override Expression StaticPropertyOrField(Type type, string name)
		{
			return Binder.StaticPropertyOrField(type,name);
		}

		public override Expression InstanceCall(Expression instance, string name, IEnumerable<Expression> parameters)
		{
			return Binder.InstanceCall(instance,name,parameters);
		}

		public override Expression InstancePropertyOrFieldWithArgs(Expression instance, string name, IEnumerable<Expression> indexes)
		{
			return Binder.InstancePropertyOrField(instance,name,indexes);
		}

		public override Expression InstancePropertyOrField(Expression instance, string name)
		{
			return Binder.InstancePropertyOrField(instance,name);
		}

		public override Expression Ternary(Expression condition, Expression ifTrue, Expression ifFalse)
		{
			condition=ToBoolean(condition);
			
			Type mostSpecificType=TypeSupport.MostSpecificType(ifTrue.Type,ifFalse.Type);
			if(mostSpecificType==null) mostSpecificType=typeof(object);

			ifTrue=ifTrue.ConvertTo(mostSpecificType);
			ifFalse=ifFalse.ConvertTo(mostSpecificType);

			return Expression.Condition(condition,ifTrue,ifFalse);
		}

		public override Expression Like(CaseMode caseMode, Expression lhs, Expression pattern)
		{
			return Binder.Like(caseMode,lhs,pattern);
		}
	}
}
