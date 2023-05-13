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
	class ConvertBinder : BinderBase
	{
		private readonly Type m_Type;
		private readonly ExpressionType m_ExpressionType;

        public ConvertBinder(ExpressionType expressionType, Type type)
        {
            m_Type = type;
            m_ExpressionType = expressionType;
        }

        public override Type ReturnType
		{
			get{return m_ExpressionType == ExpressionType.TypeIs ? typeof(bool) : m_Type;}
		}

        public override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
        {
            var restrictions = BindingRestrictions.Empty;
            Expression toConvert = target.GetLimitedExpression();

            Expression? expression = null;

            switch(m_ExpressionType)
            {
                case ExpressionType.TypeAs:
                    expression = Expression.TypeAs(toConvert, m_Type);
                    break;

                case ExpressionType.Convert:
                    expression = Expression.Convert(toConvert, m_Type);
                    break;

                case ExpressionType.TypeIs:
                    expression = Expression.TypeIs(toConvert, m_Type);
                    break;

                default:
                    throw new Exception("unsupported expression type");
            }

            restrictions = restrictions.AndLimitType(target);
            return new DynamicMetaObject(expression, restrictions);
        }
    }
}
