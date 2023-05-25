using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Arrow.Dynamic
{
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Converts an expression to the specified type.
        /// If the expression if already of the specified type then no conversion is done
        /// </summary>
        /// <typeparam name="T">The type to convert to</typeparam>
        /// <param name="expression">The expression to convert</param>
        /// <returns>A converted expression</returns>
        public static Expression ConvertTo<T>(this Expression expression)
        {
            return expression.Type == typeof(T) ? expression : Expression.Convert(expression, typeof(T));
        }

        /// <summary>
        /// Converts an expression to the specified type
        /// If the expression if already of the specified type then no conversion is done
        /// </summary>
        /// <param name="expression">The expression to convert</param>
        /// <param name="type">The type to convert to</param>
        /// <returns>A converted expression</returns>
        public static Expression ConvertTo(this Expression expression, Type type)
        {
            return expression.Type == type ? expression : Expression.Convert(expression, type);
        }

        /// <summary>
        /// Checks to see if an expression is of a given type.
        /// This saves writing long winded if statements
        /// </summary>
        /// <typeparam name="T">The type to check for</typeparam>
        /// <param name="expression">The expression to check</param>
        /// <returns></returns>
        public static bool IsOfType<T>(this Expression expression)
        {
            return expression.Type == typeof(T);
        }
    }
}
