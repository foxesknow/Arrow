using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Arrow.Dynamic
{
	/// <summary>
	/// Useful functions for dealing with numeric types
	/// </summary>
	class Numbers
	{
		/// <summary>
		/// Indicates if an expression represents a numeric type
		/// </summary>
		/// <param name="expression">The expression to check</param>
		/// <returns>true if numeric, otherwise false</returns>
		public static bool IsNumeric(Expression expression)
		{
			if(expression==null) throw new ArgumentNullException("expression");

			TypeCode typeCode=Type.GetTypeCode(expression.Type);
			return IsNumeric(typeCode);
		}

		/// <summary>
		/// Indicates if a type represents a numeric type
		/// </summary>
		/// <param name="type">The type to check</param>
		/// <returns>true if numeric, otherwise false</returns>
		public static bool IsNumeric(Type type)
		{
			if(type==null) throw new ArgumentNullException("type");

			TypeCode typeCode=Type.GetTypeCode(type);
			return IsNumeric(typeCode);
		}

		/// <summary>
		/// Indicates if a type code represents a numeric type
		/// </summary>
		/// <param name="typeCode">The code to check</param>
		/// <returns>true if numeric, otherwise false</returns>
		public static bool IsNumeric(TypeCode typeCode)
		{
			switch(typeCode)
			{
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
					return true;

				default:
					return false;
			}
		}
	}
}
