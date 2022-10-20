using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

using Arrow.Scripting;
using Arrow.Dynamic;
using Arrow.Reflection;

namespace Arrow.Scripting.Wire.StaticExpression
{
	class StaticExpressionFactory : ExpressionFactory
	{
		protected static readonly MethodInfo RegexIsMatch;

		static StaticExpressionFactory()
		{
			RegexIsMatch=typeof(Regex).GetMethod("IsMatch",new[]{typeof(string),typeof(string)})!;
		}

		public StaticExpressionFactory(Parser parser) : base(parser)
		{
		}

		public override Expression LogicalAnd(Expression lhs, Expression rhs)
		{
			m_Parser.RequireType<bool>(lhs,"logical and");
			m_Parser.RequireType<bool>(rhs,"logical and");

			return Expression.AndAlso(lhs,rhs);
		}

		public override Expression LogicalOr(Expression lhs, Expression rhs)
		{
			m_Parser.RequireType<bool>(lhs,"logical or");
			m_Parser.RequireType<bool>(rhs,"logical or");

			return Expression.OrElse(lhs,rhs);
		}

		public override Expression BitwiseAnd(Expression lhs, Expression rhs)
		{
			return Expression.And(lhs, rhs);
		}

		public override Expression BitwiseOr(Expression lhs, Expression rhs)
		{
			return Expression.Or(lhs, rhs);
		}

		public override Expression NullCoalesce(Expression lhs, Expression rhs)
		{
			NormalizeBinaryExpression(ref lhs,ref rhs);

			if(lhs.Type.IsValueType || rhs.Type.IsValueType)
			{
				throw m_Parser.MakeException("null coalesce can only be applied to reference types");
			}

			return Expression.Coalesce(lhs,rhs);
		}

		public override Expression Equal(CaseMode caseMode, Expression lhs, Expression rhs)
		{
			NormalizeBinaryExpression(ref lhs, ref rhs);
			return ExpressionEx.Equal(caseMode,lhs,rhs);
		}

		public override Expression RegexEqual(Expression lhs, Expression rhs)
		{
			NormalizeBinaryExpression(ref lhs, ref rhs);
			if(lhs.Type!=typeof(string)) throw m_Parser.MakeException("left side of regex must be string");
			if(rhs.Type!=typeof(string)) throw m_Parser.MakeException("right side of regex must be string");

			return Expression.Call(null,RegexIsMatch,lhs,rhs);
		}

		public override Expression NotEqual(CaseMode caseMode, Expression lhs, Expression rhs)
		{
			NormalizeBinaryExpression(ref lhs, ref rhs);
			return ExpressionEx.NotEqual(caseMode,lhs,rhs);
		}

		public override Expression GreaterThan(Expression lhs, Expression rhs)
		{
			NormalizeBinaryExpression(ref lhs, ref rhs);
			return Expression.GreaterThan(lhs,rhs);
		}

		public override Expression GreaterThanOrEqual(Expression lhs, Expression rhs)
		{
			NormalizeBinaryExpression(ref lhs, ref rhs);
			return Expression.GreaterThanOrEqual(lhs,rhs);
		}

		public override Expression LessThan(Expression lhs, Expression rhs)
		{
			NormalizeBinaryExpression(ref lhs, ref rhs);
			return Expression.LessThan(lhs,rhs);
		}

		public override Expression LessThanOrEqual(Expression lhs, Expression rhs)
		{
			NormalizeBinaryExpression(ref lhs, ref rhs);
			return Expression.LessThanOrEqual(lhs,rhs);
		}


		public override Expression Add(Expression lhs, Expression rhs)
		{
			Expression? expression=null;

			if(lhs.IsOfType<string>() && rhs.IsOfType<string>())
			{
				expression=StringExpression.Concat(lhs,rhs);
			}
			else
			{			
				NormalizeBinaryExpression(ref lhs,ref rhs);
				expression=Expression.Add(lhs,rhs);
			}

			return expression;
		}

		public override Expression Subtract(Expression lhs, Expression rhs)
		{
			NormalizeBinaryExpression(ref lhs,ref rhs);
			return Expression.Subtract(lhs,rhs);
		}

		public override Expression Multiply(Expression lhs, Expression rhs)
		{
			NormalizeBinaryExpression(ref lhs,ref rhs);
			return Expression.Multiply(lhs,rhs);
		}

		public override Expression Divide(Expression lhs, Expression rhs)
		{
			NormalizeBinaryExpression(ref lhs,ref rhs);
			return Expression.Divide(lhs,rhs);
		}

		public override Expression Modulo(Expression lhs, Expression rhs)
		{
			NormalizeBinaryExpression(ref lhs,ref rhs);
			return Expression.Modulo(lhs,rhs);
		}

		public override Expression Not(Expression target)
		{
			m_Parser.RequireType<bool>(target,"not");
			return Expression.Not(target);
		}

		public override Expression UnaryPlus(Expression target)
		{
			return target;
		}

		public override Expression UnaryMinus(Expression target)
		{
			return Expression.Negate(target);
		}

		public override Expression ArrayAccess(Expression target, IEnumerable<Expression> bounds)
		{
			Expression? expression=null;

			if(ExpressionEx.TryArrayAccess(target,bounds,out expression)==false)
			{
				throw m_Parser.MakeException("could not resolve array");
			}
			return expression;
		}

		public override Expression Cast(Expression target, Type type)
		{
			return Expression.Convert(target,type);
		}

		public override Expression Is(Expression target, Type type)
		{
			return Expression.TypeIs(target,type);
		}

		public override Expression As(Expression target, Type type)
		{
			return Expression.TypeAs(target,type);
		}

		public override Expression ToBoolean(Expression target)
		{
			if(target.IsOfType<bool>()==false) throw m_Parser.MakeException("not a boolean");
			return target;
		}

		/// <summary>
		/// Normalizes two expression prior to a binary operation.
		/// This typically involves checking to see if a type conversion
		/// is required, and if so adjusting the expressions
		/// </summary>
		/// <param name="lhs">The left side of the binary operation</param>
		/// <param name="rhs">The right side of the binary operation</param>
		private void NormalizeBinaryExpression(ref Expression lhs, ref Expression rhs)
		{
			if(TypeCoercion.NormalizeBinaryExpression(ref lhs, ref rhs)==false)
			{
				throw m_Parser.MakeException("could not find a common type");
			}
		}

		public override Expression StaticCall(Type type, string name, IEnumerable<Expression> parameters)
		{
			var expression=MethodCallResolver.Call(type,name,StaticFlags,parameters.ToArray());
			return expression;
		}

		public override Expression StaticPropertyOrFieldWithArgs(Type type, string name, IEnumerable<Expression> indexes)
		{
			if(ExpressionEx.TryPropertyOrFieldWithArguments(type,name,StaticFlags,indexes,out var expression)) return expression;
			
			throw this.Parser.MakeException("could not resolve static array access");
		}

		public override Expression StaticPropertyOrField(Type type, string name)
		{
			Expression expression=ExpressionEx.PropertyOrField(type,name,StaticFlags);
			return expression;
		}

		public override Expression InstanceCall(Expression instance, string name, IEnumerable<Expression> parameters)
		{
			var expression=MethodCallResolver.Call(instance,name,InstanceFlags,parameters.ToArray());
			return expression;
		}

		public override Expression InstancePropertyOrFieldWithArgs(Expression instance, string name, IEnumerable<Expression> indexes)
		{
			if(ExpressionEx.TryPropertyOrFieldWithArguments(instance,name,InstanceFlags,indexes,out var expression)) return expression;
			
			throw this.Parser.MakeException("could not resolve instance array access");
		}

		public override Expression InstancePropertyOrField(Expression instance, string name)
		{
			var expression=ExpressionEx.PropertyOrField(instance,name,InstanceFlags);
			return expression;
		}

		public override Expression Ternary(Expression condition, Expression ifTrue, Expression ifFalse)
		{
			this.Parser.RequireType<bool>(condition,"ternary");			
			return Expression.Condition(condition,ifTrue,ifFalse);
		}

		public override Expression Like(CaseMode caseMode, Expression lhs, Expression pattern)
		{
			Parser.RequireType<string>(lhs,"like");
			Parser.RequireType<string>(pattern,"like");
			
			return LikeEvaluator.Like(caseMode,lhs,pattern);
		}
	}
}
