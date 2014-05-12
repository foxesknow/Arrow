using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Dynamic;

using Arrow.Dynamic;
using Arrow.Scripting;

namespace Arrow.Scripting.Wire.DynamicExpression.Binders
{
	class RelationalBinder : BinaryOperationBinder
	{
		private readonly Func<Expression,Expression,Expression> m_Factory;

		public RelationalBinder(Func<Expression,Expression,Expression> factory, ExpressionType expressionType) : base(expressionType)
		{
			m_Factory=factory;
		}


		public override DynamicMetaObject FallbackBinaryOperation(DynamicMetaObject target, DynamicMetaObject arg, DynamicMetaObject errorSuggestion)
		{
			var restrictions=BindingRestrictions.Empty;
			Expression expression=null;

			var lhs=target.GetLimitedExpression();
			var rhs=arg.GetLimitedExpression();

			if(TypeCoercion.NormalizeBinaryExpression(ref lhs, ref rhs))
			{
				expression=m_Factory(lhs,rhs);
			}
			else
			{
				expression=this.ThrowException("RelationalBinder: could not normalize expressions");
			}

			// We need to convert it to an object, as thats what BinaryOperationBinder returns
			expression=expression.ConvertTo<object>();

			restrictions=restrictions.AndLimitType(target).AndLimitType(arg);			

			return new DynamicMetaObject(expression,restrictions);
		}
	}
}
