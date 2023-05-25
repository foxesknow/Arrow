using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Dynamic;

namespace Arrow.Dynamic
{
    /// <summary>
    /// Useful DynamicMetaObject extensions
    /// </summary>
    public static class DynamicMetaObjectExtensions
    {
        /// <summary>
        /// Returns the expression held in the meta object which the appropriate
        /// conversions necessary to make its type equal the LimitType
        /// </summary>
        /// <param name="meta">The object whose expression will be extracted</param>
        /// <returns>An expression</returns>
        public static Expression GetLimitedExpression(this DynamicMetaObject meta)
        {
            Expression expression = meta.Expression;
            Type limitType = meta.LimitType;

            if(limitType.IsValueType && expression.Type == typeof(object))
            {
                expression = Expression.Unbox(expression, limitType);
            }

            expression = expression.ConvertTo(limitType);

            return expression;
        }
    }
}
