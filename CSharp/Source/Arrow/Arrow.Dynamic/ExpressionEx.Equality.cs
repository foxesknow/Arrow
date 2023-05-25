using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Arrow.Scripting;

namespace Arrow.Dynamic
{
    public static partial class ExpressionEx
    {
        public static Expression Equal(CaseMode caseMode, Expression lhs, Expression rhs)
        {
            if(lhs == null) throw new ArgumentNullException("lhs");
            if(rhs == null) throw new ArgumentNullException("rhs");

            Expression? expression = null;

            if(caseMode == CaseMode.Insensitive && AreStrings(lhs, rhs))
            {
                expression = StringExpression.Equal(caseMode, lhs, rhs);
            }
            else
            {
                expression = Expression.Equal(lhs, rhs);
            }

            return expression;
        }

        public static Expression NotEqual(CaseMode caseMode, Expression lhs, Expression rhs)
        {
            if(lhs == null) throw new ArgumentNullException("lhs");
            if(rhs == null) throw new ArgumentNullException("rhs");

            Expression? expression = null;

            if(caseMode == CaseMode.Insensitive && AreStrings(lhs, rhs))
            {
                expression = StringExpression.NotEqual(caseMode, lhs, rhs);
            }
            else
            {
                expression = Expression.NotEqual(lhs, rhs);
            }

            return expression;
        }

        private static bool AreStrings(Expression lhs, Expression rhs)
        {
            return lhs.Type == typeof(string) && rhs.Type == typeof(string);
        }
    }
}
