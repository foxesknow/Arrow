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
	class RelationalBinder : BinderBase
	{
		private readonly Func<Expression,Expression,Expression> m_Factory;

		public RelationalBinder(Func<Expression,Expression,Expression> factory)
		{
			m_Factory=factory;
		}

		public override Type ReturnType
		{
			get{return typeof(bool);}
		}

		public override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
		{
			var restrictions=BindingRestrictions.Empty;
			Expression expression=null;

			var lhs=target.GetLimitedExpression();
			var rhs=args[0].GetLimitedExpression();

			if(TypeCoercion.NormalizeBinaryExpression(ref lhs, ref rhs))
			{
				expression=m_Factory(lhs,rhs);
			}
			else
			{
				expression=this.ThrowException("RelationalBinder: could not normalize expressions");
			}

			restrictions=restrictions.ForAll(target,args);			
			return new DynamicMetaObject(expression,restrictions);
		}
	}
}
