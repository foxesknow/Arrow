using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Dynamic;
using System.Reflection;

using Arrow.Dynamic;
using Arrow.Scripting;
using Arrow.Reflection;

namespace Arrow.Scripting.Wire.DynamicExpression.Binders
{
	class DynamicInvokeBinder : InvokeBinder
	{
		public DynamicInvokeBinder(CallInfo callInfo) : base(callInfo)
		{

		}

		public override DynamicMetaObject FallbackInvoke(DynamicMetaObject target, DynamicMetaObject[] args, DynamicMetaObject? errorSuggestion)
		{
			var restrictions=BindingRestrictions.Empty;
			Expression? expression=null;

			// Make sure it's a delegate
			if(target.LimitType.IsDelegate()==false)
			{
				restrictions=restrictions.AndLimitType(target);
				expression=this.ThrowException("can only invoke on a delegate");

				return new DynamicMetaObject(expression,restrictions);
			}

			// The Invoke method on the delegate is prototyped
			// in the same style as the delegate
			var method=target.LimitType.GetMethod("Invoke")!;
			var arguments=args.Select(o=>o.GetLimitedExpression()).ToArray();

			int cost=0;
			var adjustedArgs=MethodCallResolver.BuildArgumentList(method,arguments,ParameterMatchType.Cast,out cost);

			if(adjustedArgs!=null)
			{
				expression=Expression.Invoke(target.GetLimitedExpression(),adjustedArgs).ConvertTo(this.ReturnType);
			}
			else
			{
				expression=this.ThrowException("could not call delegate");
			}

			restrictions=restrictions.ForAll(target,args);
			return new DynamicMetaObject(expression,restrictions);
		}
	}
}
