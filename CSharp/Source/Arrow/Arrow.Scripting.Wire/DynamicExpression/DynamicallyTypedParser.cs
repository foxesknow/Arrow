using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Arrow.Compiler;
using Arrow.Scripting;
using Arrow.Collections;
using Arrow.Reflection;
using Arrow.Dynamic;

using Arrow.Scripting.Wire.DynamicExpression.Binders;

namespace Arrow.Scripting.Wire.DynamicExpression
{
	class DynamicallyTypedParser : Parser
	{
        private static readonly ParameterExpression s_LambdaParameter = Expression.Parameter(typeof(IVariableRead));
        private static readonly System.Reflection.MethodInfo GetVariableMethod = typeof(IVariableRead).GetMethod("GetVariable")!;

        private readonly DynamicParseContext m_DynamicParseContext;

        public DynamicallyTypedParser(ITokenizer tokenizer, DynamicParseContext parseContext) : base(tokenizer, parseContext)
        {
            m_DynamicParseContext = parseContext;
            this.ExpressionFactory = new DynamicExpressionFactory(this);
        }

        public LambdaExpression GenerateLambda()
        {
            var expression = GenerateExpression();
            var lambda = Expression.Lambda(expression, s_LambdaParameter);
            return lambda;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">The return type of the function to generate</typeparam>
        /// <returns></returns>
        public Expression<T> GenerateLambda<T>()
        {
            var expression = GenerateExpression();

            // Coerce to a boolean if that what the user wanted
            if(typeof(T) == typeof(bool)) expression = this.ExpressionFactory.ToBoolean(expression);

            return Expression.Lambda<T>(expression, s_LambdaParameter);
        }

        /// <summary>
        /// Called when we encounter something that looks like a symbol. Eg
        /// 
        ///   foo()		a free standing function
        ///   foo		either a type of a variable within the scope
        /// </summary>
        /// <param name="symbolName"></param>
        /// <returns></returns>
        protected override Expression Symbol(string symbolName)
        {
            Expression? expression = null;

            if(m_Tokenizer.Current.ID == TokenID.LeftParen)
            {
                throw MakeException("Free standing functions not supported (yet!)");
            }
            else
            {
                Type? type = null;

                if(m_DynamicParseContext.AlwaysTreatAsSymbol.Contains(symbolName) == false)
                {                
                    type = ResolveType(symbolName);
                }

                if(type != null)
                {
                    // The name is actually a type, so it's some sort of static access
                    expression = StaticAccess(type);
                }
                else
                {
                    // We just need to call into the scope to grab the value
                    expression = Expression.Call(s_LambdaParameter, GetVariableMethod, Expression.Constant(symbolName));
                }
            }

            return expression;
        }
    }
}
