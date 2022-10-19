using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Arrow.Dynamic
{
	/// <summary>
	/// Useful expression methods
	/// </summary>
	public static partial class ExpressionEx
	{
		/// <summary>
		/// Generate code to throw an exception via its default constructor
		/// </summary>
		/// <typeparam name="T">The type of exception to throw</typeparam>
		/// <returns>An expression that will throw an exception</returns>
		public static UnaryExpression Throw<T>() where T:Exception
		{
			var toThrow=Expression.Throw(Expression.New(typeof(T)));
			return toThrow;
		}

		/// <summary>
		/// Generate code to throw an exception via its message constructor
		/// </summary>
		/// <typeparam name="T">The type of exception to throw</typeparam>
		/// <param name="message">The message to pass to the exception constructor</param>
		/// <returns>An expression that will throw an exception</returns>
		public static UnaryExpression Throw<T>(string message) where T:Exception
		{
			var ctor=typeof(T).GetConstructor(TypeArray.Make<string>());

			var toThrow=Expression.Throw(Expression.New(ctor!,Expression.Constant(message)));
			return toThrow;
		}

		/// <summary>
		/// Generates code to throw an exception in a block that returns a value
		/// </summary>
		/// <typeparam name="T">The type of exception to throw</typeparam>
		/// <typeparam name="R">The return type for the block</typeparam>
		/// <param name="message">The message to pass to the exception constructor</param>
		/// <param name="returnValue">The value the block will return</param>
		/// <returns>An expression that will throw an exception</returns>
		public static Expression ThrowAndReturn<T,R>(string message, R returnValue) where T:Exception
		{
			var ctor=typeof(T).GetConstructor(TypeArray.Make<string>());

			var e=Expression.Throw(Expression.New(ctor!,Expression.Constant(message)));
			return Expression.Block(e,Expression.Constant(returnValue));
		}
	}
}
