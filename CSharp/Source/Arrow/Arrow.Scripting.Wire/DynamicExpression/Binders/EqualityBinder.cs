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
	class EqualityBinder : BinaryOperationBinder
	{
		private readonly CaseMode m_CaseMode;
		private readonly ExpressionType m_ExpressionType;

		public EqualityBinder(ExpressionType expressionType, CaseMode caseMode) : base(expressionType)
		{
			m_ExpressionType=expressionType;
			m_CaseMode=caseMode;
		}

		public override DynamicMetaObject FallbackBinaryOperation(DynamicMetaObject target, DynamicMetaObject arg, DynamicMetaObject errorSuggestion)
		{
			var restrictions=BindingRestrictions.Empty;
			Expression expression=null;

			var lhs=target.GetLimitedExpression();
			var rhs=arg.GetLimitedExpression();

			if(lhs.IsOfType<string>() && rhs.IsOfType<string>())
			{
				expression=(m_ExpressionType==ExpressionType.Equal ? StringExpression.Equal(m_CaseMode,lhs,rhs) : StringExpression.NotEqual(m_CaseMode,lhs,rhs));				
			}
			else
			{
				if(TypeCoercion.NormalizeBinaryExpression(ref lhs, ref rhs))
				{
					expression=(m_ExpressionType==ExpressionType.Equal ? Expression.Equal(lhs,rhs) : Expression.NotEqual(lhs,rhs));
				}
				else
				{
					expression=this.ThrowException("EqualityBinder: could not normalize expressions");
				}
			}

			// We need to convert it to an object, as thats what BinaryOperationBinder returns
			expression=expression.ConvertTo<Object>();

			restrictions=restrictions.AndLimitType(target).AndLimitType(arg);
			
			return new DynamicMetaObject(expression,restrictions);
		}
	}
}
