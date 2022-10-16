using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Dynamic;

using Arrow.Dynamic;
using Arrow.Scripting;

namespace Arrow.Scripting.Wire.DynamicExpression.Binders
{
	class ToBinaryBinder : BinderBase
	{
		public override Type ReturnType
		{
			get{return typeof(bool);}
		}

		public override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
		{
			var restrictions=BindingRestrictions.Empty.AndLimitType(target);
			var value=target.GetLimitedExpression();

			Expression? expression=value;

			if(value.Type!=typeof(bool))
			{
				Type actualType=target.LimitType;
				var toBoolean=typeof(Convert).GetMethod("ToBoolean",new Type[]{actualType});

				if(toBoolean!=null)
				{
					// For reference types do a test for null
					expression=Expression.Call(toBoolean,value.ConvertTo<object>());
				}
				else
				{
					expression=this.ThrowException("ToBinaryBinder: could not convert expression to boolean");
				}
			}

			return new DynamicMetaObject(expression,restrictions);
		}
	}
}
