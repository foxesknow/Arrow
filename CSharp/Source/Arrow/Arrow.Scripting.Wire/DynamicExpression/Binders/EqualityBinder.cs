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
	class EqualityBinder : BinderBase
	{
		private readonly CaseMode m_CaseMode;
		private readonly ExpressionType m_ExpressionType;

		public EqualityBinder(ExpressionType expressionType, CaseMode caseMode)// : base(expressionType)
		{
			m_ExpressionType=expressionType;
			m_CaseMode=caseMode;
		}

		public override Type ReturnType
		{
			get{return typeof(bool);}
		}

		public override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)//, DynamicMetaObject errorSuggestion)
		{
			var restrictions=BindingRestrictions.Empty;
			Expression? expression=null;

			var lhs=target.GetLimitedExpression();
			var rhs=args[0].GetLimitedExpression();

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

			restrictions=restrictions.AndLimitType(target).AndLimitType(args[0]);			
			
			return new DynamicMetaObject(expression,restrictions);
		}
	}
}
