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
	class DynamicGetIndexBinder : GetIndexBinder
	{
		private readonly string? m_Name;
		private readonly BindingFlags m_BindingFlags;

        public DynamicGetIndexBinder(CallInfo callInfo) : this(null, BindingFlags.Default, callInfo)
        {

        }

        public DynamicGetIndexBinder(string? name, BindingFlags bindingFlags, CallInfo callInfo) : base(callInfo)
        {
            m_Name = name;
            m_BindingFlags = bindingFlags;
        }

        public override DynamicMetaObject FallbackGetIndex(DynamicMetaObject target, DynamicMetaObject[] indexes, DynamicMetaObject? errorSuggestion)
        {
            if(target.Value is IDynamicMetaObjectProvider)
            {
                return DeferToDynamicMetaObjectProvider(target, indexes, errorSuggestion);
            }

            BindingRestrictions restrictions = BindingRestrictions.Empty;
            Expression? expression = null;

            var instance = target.GetLimitedExpression();

            if(m_Name == null)
            {
                var i = indexes.Select(o => o.GetLimitedExpression());
                ExpressionEx.TryArrayAccess(instance, i, out expression);
            }
            else
            {
                var i = indexes.Select(o => o.GetLimitedExpression());
                ExpressionEx.TryPropertyOrFieldWithArguments(instance, m_Name, m_BindingFlags, i, out expression);
            }

            if(expression != null)
            {
                expression = expression.ConvertTo(this.ReturnType);
            }
            else
            {
                expression = this.ThrowException("could not access array");
            }

            restrictions = restrictions.ForAll(target, indexes);
            return new DynamicMetaObject(expression, restrictions);
        }

        private DynamicMetaObject DeferToDynamicMetaObjectProvider(DynamicMetaObject target, DynamicMetaObject[] indexes, DynamicMetaObject? errorSuggestion)
        {
            // We've got an expression such as provider.member[index]
            // and it's in a dynamic type, so we'll need to resolve
            // the member and then have another go at doing the index access
            var getInstance = Binder.InstancePropertyOrField(target.GetLimitedExpression(), m_Name!);

            var i = indexes.Select(o => o.GetLimitedExpression());
            var arrayLookup = Binder.ArrayAccess(getInstance, i);
            return new DynamicMetaObject(arrayLookup, target.Restrictions.ForAll(target, indexes));
        }
    }
}
