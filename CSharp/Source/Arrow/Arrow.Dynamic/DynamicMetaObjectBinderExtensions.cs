using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Dynamic;

namespace Arrow.Dynamic
{
	public static class DynamicMetaObjectBinderExtensions
	{
		public static Expression ThrowException(this DynamicMetaObjectBinder binder, string message)
		{
			var thrower=ExpressionEx.Throw<DynamicException>(message);
			var defaultForReturn=Expression.Default(binder.ReturnType);

			return Expression.Block(thrower,defaultForReturn);
		}

		public static Expression ThrowException<T>(this DynamicMetaObjectBinder binder, string message) where T:Exception
		{
			var thrower=ExpressionEx.Throw<T>(message);
			var defaultForReturn=Expression.Default(binder.ReturnType);

			return Expression.Block(thrower,defaultForReturn);
		}
	}
}
