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
	class DynamicGetMemberBinder : GetMemberBinder
	{
		private readonly BindingFlags m_BindingFlags;

		public DynamicGetMemberBinder(string name, BindingFlags bindingFlags) : base(name,true)
		{
			m_BindingFlags=bindingFlags;
		}

		public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
		{
			if(target.Value is IDynamicMetaObjectProvider)
			{
				return DeferToDynamicMetaObjectProvider(target,errorSuggestion);
			}

			BindingRestrictions restrictions=BindingRestrictions.Empty;
			Expression expression=null;

			var instance=target.GetLimitedExpression();

			MemberExpression memberExpression=null;
			ExpressionEx.TryPropertyOrField(instance,this.Name,m_BindingFlags,out memberExpression);
			expression=memberExpression;

			if(expression!=null)
			{
				expression=expression.ConvertTo(this.ReturnType);
			}
			else
			{
				expression=this.ThrowException("could not resolve instance property or field");
			}

			return new DynamicMetaObject(expression,restrictions);
		}

		private DynamicMetaObject DeferToDynamicMetaObjectProvider(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
		{
			// We don't add any additional members to the provider,
			// so we'll just opt to throw an exception
			return new DynamicMetaObject(this.ThrowException("could not find "+this.Name),target.Restrictions.AndLimitType(target));
		}
	}
}
