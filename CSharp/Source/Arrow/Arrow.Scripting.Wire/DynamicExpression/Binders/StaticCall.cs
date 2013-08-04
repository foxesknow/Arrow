using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Dynamic;
using System.Reflection;

using Arrow.Dynamic;
using Arrow.Scripting;

namespace Arrow.Scripting.Wire.DynamicExpression.Binders
{
	class StaticCall : BinderBase
	{
		private readonly Type m_TargetType;
		private readonly string m_Name;
		private readonly BindingFlags m_BindingFlags;

		public StaticCall(Type targetType, string name, BindingFlags bindingFlags)
		{
			m_TargetType=targetType;
			m_Name=name;
			m_BindingFlags=bindingFlags;
		}

		public override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
		{
			BindingRestrictions restrictions=BindingRestrictions.Empty;
			Expression expression=null;

			var arguments=args.Select(o=>o.GetLimitedExpression()).ToArray();
			MethodCallExpression callExpression=null;
			if(MethodCallResolver.TryCall(m_TargetType,m_Name,m_BindingFlags,arguments,out callExpression))
			{
				expression=callExpression;
			}
			else
			{
				expression=this.ThrowException("could not resolve static call: "+m_Name);
			}

			expression=expression.ConvertTo(this.ReturnType);

			restrictions=restrictions.ForCall(null,args);
			return new DynamicMetaObject(expression,restrictions);
		}
	}
}
