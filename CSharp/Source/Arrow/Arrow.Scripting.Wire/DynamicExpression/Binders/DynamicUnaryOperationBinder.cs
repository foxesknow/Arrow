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
	class DynamicUnaryOperationBinder : UnaryOperationBinder
	{
		public DynamicUnaryOperationBinder(ExpressionType operation) : base(operation)
		{
		}

        public override DynamicMetaObject FallbackUnaryOperation(DynamicMetaObject target, DynamicMetaObject? errorSuggestion)
        {
            var restrictions = BindingRestrictions.Empty;
            Expression? expression = null;

            var instance = target.GetLimitedExpression();
            expression = Expression.MakeUnary(this.Operation, instance, null!);

            expression = expression.ConvertTo(this.ReturnType);
            restrictions = restrictions.AndLimitType(target);

            return new DynamicMetaObject(expression, restrictions);
        }
    }
}
