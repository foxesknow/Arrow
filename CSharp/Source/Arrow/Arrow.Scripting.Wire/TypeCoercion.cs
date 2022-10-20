using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Arrow.Dynamic;
using Arrow.Reflection;

namespace Arrow.Scripting.Wire
{
	static class TypeCoercion
	{
		/// <summary>
		/// Normalizes two expression prior to a binary operation.
		/// This typically involves checking to see if a type conversion
		/// is required, and if so adjusting the expressions
		/// </summary>
		/// <param name="lhs">The left side of the binary operation</param>
		/// <param name="rhs">The right side of the binary operation</param>
		public static bool NormalizeBinaryExpression(ref Expression lhs, ref Expression rhs)
		{
			bool normalized=false;

			if(NumbersCLS.IsNumeric(lhs) && NumbersCLS.IsNumeric(rhs))
			{
				TypeCode left=Type.GetTypeCode(lhs.Type);
				TypeCode right=Type.GetTypeCode(rhs.Type);

				Type promotionType=(left>right ? lhs.Type : rhs.Type);

				lhs=Promote(lhs,promotionType);
				rhs=Promote(rhs,promotionType);

				normalized=true;
			}
			else
			{
				var commonType=TypeSupport.MostSpecificType(lhs.Type,rhs.Type);
				if(commonType!=null)
				{
					lhs=lhs.ConvertTo(commonType);
					rhs=rhs.ConvertTo(commonType);

					normalized=true;
				}
			}

			return normalized;
		}

		public static void NormalizeNumbericBinaryExpression(ref Expression lhs, ref Expression rhs)
		{
			if(NumbersCLS.IsNumeric(lhs) && NumbersCLS.IsNumeric(rhs))
			{
				TypeCode left=Type.GetTypeCode(lhs.Type);
				TypeCode right=Type.GetTypeCode(rhs.Type);

				Type promotionType=(left>right ? lhs.Type : rhs.Type);

				lhs=Promote(lhs,promotionType);
				rhs=Promote(rhs,promotionType);
			}
		}

		/// <summary>
		/// Type promotes an expression
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Expression Promote(Expression expression, Type type)
		{
			return expression.ConvertTo(type);
		}
	}
}
