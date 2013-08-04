using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.IO;

using Arrow.Dynamic;

namespace Arrow.Scripting.Wire
{
	/// <summary>
	/// Generates code for an expression
	/// <typeparam name="T">The type of the parse context to use</typeparam>
	/// </summary>
	public abstract class CodeGenerator<T> where T:ParseContext
	{
		protected CodeGenerator()
		{
		}

		/// <summary>
		/// Compiles an expression into a LambdaExpression
		/// </summary>
		/// <param name="expression">The expression to parse</param>
		/// <param name="context">Context information for the expression</param>
		/// <returns>A lambda expression representing the expression and any parameters in the context</returns>
		public abstract LambdaExpression CreateLambda(string expression, T context);
		

		/// <summary>
		/// Compiles an expression to a general purpose delegate
		/// </summary>
		/// <param name="expression">The expression to parse</param>
		/// <param name="context">Context information for the expression</param>
		/// <returns>A delegate for the expression</returns>
		public abstract Delegate CreateDelegate(string expression, T context);
	}
}
