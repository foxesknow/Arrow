using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.IO;

namespace Arrow.Scripting.Wire.StaticExpression
{
	/// <summary>
	/// Generates static expressions
	/// </summary>
	public class StaticCodeGenerator : CodeGenerator<StaticParseContext>
	{
        /// <summary>
        /// Compiles an expression into a LambdaExpression
        /// </summary>
        /// <param name="expression">The expression to parse</param>
        /// <param name="context">Context information for the expression</param>
        /// <returns>A lambda expression representing the expression and any parameters in the context</returns>
        public override LambdaExpression CreateLambda(string expression, StaticParseContext context)
        {
            if(expression == null) throw new ArgumentNullException("expression");
            if(context == null) throw new ArgumentNullException("context");

            using(StringReader reader = new StringReader(expression))
            {
                Tokenizer tokenizer = new Tokenizer(reader, "<no file>");
                tokenizer.Initialize();

                var parser = new StaticallyTypedParser(tokenizer, context);
                return parser.GenerateLambda();
            }
        }

        /// <summary>
        /// Compiles an expression into a typed DLR expression
        /// </summary>
        /// <param name="expression">The expression to parse</param>
        /// <param name="context">Context information for the expression</param>
        /// <returns>A typed expression representing the expression and any parameters in the context</returns>
        public Expression<T> CreateLambda<T>(string expression, StaticParseContext context)
        {
            if(expression == null) throw new ArgumentNullException("expression");
            if(context == null) throw new ArgumentNullException("context");

            using(StringReader reader = new StringReader(expression))
            {
                Tokenizer tokenizer = new Tokenizer(reader, "<no file>");
                tokenizer.Initialize();

                var parser = new StaticallyTypedParser(tokenizer, context);
                return parser.GenerateLambda<T>();
            }
        }

        /// <summary>
        /// Compiles an expression to a general purpose delegate
        /// </summary>
        /// <param name="expression">The expression to parse</param>
        /// <param name="context">Context information for the expression</param>
        /// <returns>A delegate for the expression</returns>
        public override Delegate CreateDelegate(string expression, StaticParseContext context)
        {
            var lambda = CreateLambda(expression, context);

            Delegate function = lambda.Compile();
            return function;
        }

        /// <summary>
        /// Compiles a function to a typed delegate
        /// </summary>
        /// <typeparam name="T">A delegate type that matches the signature of the expression</typeparam>
        /// <param name="expression">The expression to parse</param>
        /// <param name="context">Context information for the expression</param>
        /// <returns>A Func instance</returns>
        public T CreateFunction<T>(string expression, StaticParseContext context)
        {
            var lambda = CreateLambda<T>(expression, context);
            var function = lambda.Compile();
            return function;
        }
    }
}
