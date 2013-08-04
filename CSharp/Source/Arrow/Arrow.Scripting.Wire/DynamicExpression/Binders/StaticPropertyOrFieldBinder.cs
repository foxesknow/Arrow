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
	class StaticPropertyOrFieldBinder : BinderBase
	{
		private readonly Type m_TargetType;
		private readonly string m_Name;
		private readonly BindingFlags m_BindingFlags;

		public StaticPropertyOrFieldBinder(Type targetType, string name, BindingFlags bindingFlags)
		{
			m_TargetType=targetType;
			m_Name=name;
			m_BindingFlags=bindingFlags;
		}

		public override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
		{
			BindingRestrictions restrictions=BindingRestrictions.Empty;
			Expression expression=null;

			if(args.Length==0)
			{
				MemberExpression memberExpression=null;
				ExpressionEx.TryPropertyOrField(m_TargetType,m_Name,m_BindingFlags,out memberExpression);
				expression=memberExpression;
			}
			else
			{
				var indexes=args.Select(o=>o.GetLimitedExpression());
				ExpressionEx.TryPropertyOrFieldWithArguments(m_TargetType,m_Name,m_BindingFlags,indexes,out expression);
			}

			if(expression!=null)
			{
				expression=expression.ConvertTo(this.ReturnType);
			}
			else
			{
				expression=this.ThrowException("could not resolve static property or field");
			}

			restrictions=restrictions.ForCall(null,args);
			return new DynamicMetaObject(expression,restrictions);
		}
	}
}
