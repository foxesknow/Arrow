using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Arrow.Reflection;
using Arrow.Dynamic;
using Arrow.Compiler;

namespace Arrow.Scripting.Wire.StaticExpression
{
	class StaticallyTypedParser : Parser
	{
        private readonly StaticParseContext m_StaticParseContext;

        public StaticallyTypedParser(ITokenizer tokenizer, StaticParseContext parseContext) : base(tokenizer, parseContext)
        {
            m_StaticParseContext = parseContext;
            this.ExpressionFactory = new StaticExpressionFactory(this);
        }

        public LambdaExpression GenerateLambda()
        {
            var expression = GenerateExpression();
            var lambda = Expression.Lambda(expression, m_StaticParseContext.Parameters);
            return lambda;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">The type of the delegate to generate</typeparam>
        /// <returns></returns>
        public Expression<T> GenerateLambda<T>()
        {
            // Make sure it's a delegate...
            if(typeof(T).IsDelegate() == false) throw new ArgumentException("not a delegate type");

            // ...and grab the return type
            Type returnType = typeof(T).GetMethod("Invoke")!.ReturnType;

            var expression = GenerateExpression();

            if(expression.Type != returnType)
            {
                if(expression.Type == typeof(object) && returnType.IsValueType)
                {
                    expression = Expression.Unbox(expression, returnType);
                }
                else
                {
                    expression = expression.ConvertTo(returnType);
                }
            }

            return Expression.Lambda<T>(expression, m_StaticParseContext.Parameters);
        }

        protected override Expression Symbol(string symbolName)
        {
            // First, see if there's a parameter with a matching name
            ParameterExpression? parameter = m_StaticParseContext.Parameters.SingleOrDefault(p => string.Compare(symbolName, p.Name, true) == 0);
            if(parameter != null) return parameter;

            // Otherwise assume it's a type
            var type = ResolveType(symbolName);
            if(type == null) throw MakeException("could not resolve type: " + symbolName);

            Expression staticExpression = StaticAccess(type);

            return staticExpression;
        }
    }
}
