using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Dynamic;
using System.Reflection;

using Arrow.Dynamic;
using Arrow.Scripting;

namespace Arrow.Scripting.Wire.DynamicExpression.Binders
{
	class RegexEqualityBinder : BinderBase
	{
		protected static readonly MethodInfo RegexIsMatch;

        static RegexEqualityBinder()
        {
            RegexIsMatch = typeof(Regex).GetMethod("IsMatch", new[] { typeof(string), typeof(string) })!;
        }

        public override Type ReturnType
        {
            get{return typeof(bool);}
        }

        public override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
        {
            var restrictions = BindingRestrictions.Empty;
            Expression? expression = null;

            var lhs = target.GetLimitedExpression();
            var rhs = args[0].GetLimitedExpression();

            if(lhs.IsOfType<string>() && rhs.IsOfType<string>())
            {
                expression = Expression.Call(null, RegexIsMatch, lhs, rhs);
            }
            else
            {
                expression = this.ThrowException("RegexEqualityBinder: both sides must be strings");
            }

            restrictions = restrictions.ForAll(target, args);

            return new DynamicMetaObject(expression, restrictions);
        }
    }
}
