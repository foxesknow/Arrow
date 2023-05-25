using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using Arrow.Logging;
using Arrow.Scripting.Wire.StaticExpression;

namespace Tango.Workbench.Filters
{
    /// <summary>
    /// Compiles and manages scripts used by filter.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class StaticExpressionCompiler<TResult> : StaticExpressionCompiler
    {
        private readonly Dictionary<(Type Type, string Script), FilterScriptFunction<TResult>> m_Cache = new();

        private readonly FilterScriptFunction<TResult>? m_Default;


        public StaticExpressionCompiler() : this(null)
        {
        }

        public StaticExpressionCompiler(FilterScriptFunction<TResult>? defaultOnCompilationError)
        {
            m_Default = defaultOnCompilationError;
        }

        /// <summary>
        /// Fetches the function for the given type, compiling the script on demand if required.
        /// </summary>
        /// <param name="script"></param>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public FilterScriptFunction<TResult> GetFunction(string script, Type itemType, ILog log)
        {
            var key = (itemType, script);
            if(m_Cache.TryGetValue(key, out var function) == false)
            {
                function = Compile(script, itemType, log);
                m_Cache.Add(key, function);
            }

            return function;
        }        

        private FilterScriptFunction<TResult> Compile(string script, Type itemType, ILog log)
        {
            /*
             * The filter will receive items as objects, so we'll need to cast then
             * to the appropriate type before passing them into the script.
             * This lambda we generate will have the form:
             * 
             *   (object item, long index) => script((itemType)object, index)
             */

            try
            {
                var parseContext = MakeParseContext();
                parseContext.Parameters.Add(Expression.Parameter(itemType, "item"));
                parseContext.Parameters.Add(Expression.Parameter(typeof(long), "index"));
            
                var generator = new StaticCodeGenerator();
                var innerLambda = generator.CreateLambda(script, parseContext);

                var itemParameter = Expression.Parameter(typeof(object));
                var indexParameter = Expression.Parameter(typeof(long));

                Expression invokeInnerLambda = Expression.Invoke
                (
                    innerLambda,
                    Expression.Convert(itemParameter, itemType),
                    indexParameter
                );

                if(typeof(TResult) != invokeInnerLambda.Type)
                {
                    invokeInnerLambda = Expression.Convert(invokeInnerLambda, typeof(TResult));
                }
                
                var outerLambda = Expression.Lambda<FilterScriptFunction<TResult>>
                (
                    invokeInnerLambda,
                    itemParameter,
                    indexParameter
                );

                return outerLambda.Compile();
            }
            catch
            {
                if(m_Default is not null)
                {
                    log.Warn($"could not compile the function against the type {itemType.Name}. A default predicate will used that evaluates to null instead");

                    return m_Default;
                }
                else
                {
                    throw new WorkbenchException($"could not compile {script}");
                }
            }
        }
    }
}
