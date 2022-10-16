using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Dynamic;
using System.Runtime.CompilerServices;

using Arrow.Collections;
using Arrow.Scripting;
using Arrow.Dynamic;
using Arrow.Reflection;

namespace Arrow.Scripting.Wire.DynamicExpression.Binders
{
	public static class Binder
	{
		public static Expression ArrayAccess(Expression target, IEnumerable<Expression> indexes)
		{
			var targetAndArgs=Sequence.Cons(target,indexes);

			var binder=new DynamicGetIndexBinder(new CallInfo(indexes.Count()));
			return Expression.Dynamic(binder,binder.ReturnType,targetAndArgs);
		}

		public static Expression ToBinary(Expression target)
		{
			var binder=new ToBinaryBinder();
			return Expression.Dynamic(binder,binder.ReturnType,target);
		}

		public static Expression Equality(ExpressionType expressionType, CaseMode caseMode, Expression lhs, Expression rhs)
		{
			var binder=new EqualityBinder(expressionType,caseMode);
			return Expression.Dynamic(binder,binder.ReturnType,lhs,rhs);
		}

		public static Expression RegexEquality(Expression lhs, Expression rhs)
		{
			var binder=new RegexEqualityBinder();
			return Expression.Dynamic(binder,binder.ReturnType,lhs,rhs);
		}

		public static Expression Like(CaseMode caseMode, Expression lhs, Expression pattern)
		{
			var binder=new LikeBinder(caseMode);
			return Expression.Dynamic(binder,binder.ReturnType,lhs,pattern);
		}

		public static Expression Relational(Func<Expression,Expression,Expression> factory, ExpressionType expressionType, Expression lhs, Expression rhs)
		{
			var binder=new RelationalBinder(factory,expressionType);
			return Expression.Dynamic(binder,binder.ReturnType,lhs,rhs);
		}

		public static Expression Unary(ExpressionType operation, Expression target)
		{
			var binder=new DynamicUnaryOperationBinder(operation);
			return Expression.Dynamic(binder,binder.ReturnType,target);
		}

		public static Expression Binary(ExpressionType operation, Expression lhs, Expression rhs)
		{
			var binder=new DynamicBinaryOperationBinder(operation);
			return Expression.Dynamic(binder,binder.ReturnType,lhs,rhs);
		}

		public static Expression Convert(ExpressionType expressionType, Expression target, Type type)
		{
			var binder=new ConvertBinder(expressionType,type);
			return Expression.Dynamic(binder,binder.ReturnType,target);
		}

		public static Expression StaticCall(Type type, string methodName, IEnumerable<Expression> arguments)
		{
			var targetAndArgs=Sequence.Cons(Parser.Null,arguments);

			var binder=new StaticCall(type,methodName,ExpressionFactory.StaticFlags);
			return Expression.Dynamic(binder,binder.ReturnType,targetAndArgs);
		}

		public static Expression StaticPropertyOrField(Type type, string name)
		{			
			MemberExpression? memberExpression=null;
			ExpressionEx.TryPropertyOrField(type,name,ExpressionFactory.StaticFlags,out memberExpression);
			
			Expression? expression=memberExpression;			
			if(expression==null) expression=ExpressionEx.Throw<DynamicException>("could not find property or field: "+name);

			return expression;
		}

		public static Expression StaticPropertyOrField(Type type, string name, IEnumerable<Expression> indexes)
		{
			var targetAndArgs=Sequence.Cons(Parser.Null,indexes);
			
			var binder=new StaticPropertyOrFieldBinder(type,name,ExpressionFactory.StaticFlags);
			return Expression.Dynamic(binder,binder.ReturnType,targetAndArgs);
		}

		public static Expression InstanceCall(Expression instance, string name, IEnumerable<Expression> parameters)
		{
			var targetAndArgs=Sequence.Cons(instance,parameters);

			//var binder=new InstanceCallBinder(name,ExpressionFactory.InstanceFlags);
			var binder=new DynamicInvokeMemberBinder(name,ExpressionFactory.InstanceFlags,new CallInfo(parameters.Count()));
			return Expression.Dynamic(binder,binder.ReturnType,targetAndArgs);
		}

		public static Expression InstancePropertyOrField(Expression instance, string name)
		{
			var binder=new DynamicGetMemberBinder(name,ExpressionFactory.InstanceFlags);
			return Expression.Dynamic(binder,binder.ReturnType,Sequence.Single(instance));
		}

		public static Expression InstancePropertyOrField(Expression instance, string name, IEnumerable<Expression> indexes)
		{
			var targetAndArgs=Sequence.Cons(instance,indexes);

			var binder=new DynamicGetIndexBinder(name,ExpressionFactory.InstanceFlags,new CallInfo(indexes.Count()));
			return Expression.Dynamic(binder,binder.ReturnType,targetAndArgs);
		}
	}
}
