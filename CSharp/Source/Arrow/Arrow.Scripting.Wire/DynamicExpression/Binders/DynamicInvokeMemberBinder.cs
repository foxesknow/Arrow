using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Dynamic;
using System.Reflection;

using Arrow.Collections;
using Arrow.Dynamic;
using Arrow.Scripting;

namespace Arrow.Scripting.Wire.DynamicExpression.Binders
{
    class DynamicInvokeMemberBinder : InvokeMemberBinder
    {
        private readonly BindingFlags m_BindingFlags;

        public DynamicInvokeMemberBinder(string name, BindingFlags bindingFlags, CallInfo callInfo) : base(name, ((bindingFlags & BindingFlags.IgnoreCase) == BindingFlags.IgnoreCase), callInfo)
        {
            m_BindingFlags = bindingFlags;
        }

        public override DynamicMetaObject FallbackInvoke(DynamicMetaObject target, DynamicMetaObject[] args, DynamicMetaObject? errorSuggestion)
        {
            return DeferToDynamicMetaObjectProvider(target, args, errorSuggestion);
        }

        public override DynamicMetaObject FallbackInvokeMember(DynamicMetaObject target, DynamicMetaObject[] args, DynamicMetaObject? errorSuggestion)
        {
            if(target.Value is IDynamicMetaObjectProvider)
            {
                return DeferToDynamicMetaObjectProvider(target, args, errorSuggestion);
            }

            BindingRestrictions restrictions = BindingRestrictions.Empty;
            Expression? expression = null;

            var instance = target.GetLimitedExpression();
            var arguments = args.Select(o => o.GetLimitedExpression()).ToArray();

            MethodCallExpression? callExpression = null;
            if(MethodCallResolver.TryCall(instance, this.Name, m_BindingFlags, arguments, out callExpression))
            {
                expression = callExpression;
            }
            else
            {
                expression = this.ThrowException("cannot resolve instance call: " + this.Name);
            }

            expression = expression.ConvertTo(this.ReturnType);

            restrictions = restrictions.ForCall(target, args);
            return new DynamicMetaObject(expression, restrictions);
        }

        private DynamicMetaObject DeferToDynamicMetaObjectProvider(DynamicMetaObject target, DynamicMetaObject[] args, DynamicMetaObject? errorSuggestion)
        {
            // We've been asked to invoke something like provider.method(foo,bar).
            // Here "method" is actually a delegate, so we'll use an InvokeBinder
            // to make the call
            BindingRestrictions restrictions = target.Restrictions.Merge(BindingRestrictions.Combine(args));

            var targetAndArgs = args.Select(o => o.Expression).Prepend(target.Expression);
            var binder = new DynamicInvokeBinder(new CallInfo(args.Length));
            var expression = Expression.Dynamic(binder, typeof(object), targetAndArgs);

            return new DynamicMetaObject(expression, restrictions);
        }
    }
}
