using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Dynamic;
using System.Reflection;

using Arrow.Dynamic;

namespace Arrow.Scripting.Wire.DynamicExpression.Binders
{
	class DynamicBinaryOperationBinder : BinaryOperationBinder
	{
		public DynamicBinaryOperationBinder(ExpressionType operation) : base(operation)
		{
		}

		public override DynamicMetaObject FallbackBinaryOperation(DynamicMetaObject target, DynamicMetaObject arg, DynamicMetaObject errorSuggestion)
		{
			var restrictions=BindingRestrictions.Empty;
			Expression expression=null;

			var lhs=target.GetLimitedExpression();
			var rhs=arg.GetLimitedExpression();

			if(lhs.IsOfType<string>() && rhs.IsOfType<string>() && this.Operation==ExpressionType.Add)
			{
				expression=StringExpression.Concat(lhs,rhs);
			}
			else if(TypeCoercion.NormalizeBinaryExpression(ref lhs, ref rhs))
			{
				expression=Expression.MakeBinary(this.Operation,lhs,rhs);
			}
			else
			{
				expression=this.ThrowException("BinaryBinder: could not normalize expressions");
			}

			expression=expression.ConvertTo<object>();
			restrictions=restrictions.AndLimitType(target).AndLimitType(arg);

			return new DynamicMetaObject(expression,restrictions);
		}
	}
}
