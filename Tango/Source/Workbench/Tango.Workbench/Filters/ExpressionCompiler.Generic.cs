using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using Arrow.Logging;
using Arrow.Scripting.Wire.StaticExpression;
using Arrow.Scripting.Wire.DynamicExpression;
using Arrow.Scripting;
using System.Dynamic;
using Tango.Workbench.Data;

namespace Tango.Workbench.Filters
{
    /// <summary>
    /// Compiles and manages expressions used by filters.
    /// 
    /// NOTE: Expression that start with a @ are treated as dynamic expressions.
    /// The @ will be removed and the expression compiled. 
    /// At runtime if the "item" in a StructuredObject it is converted to an ExpandoObject
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public sealed class ExpressionCompiler<TResult> : ExpressionCompiler
    {
        private readonly Dictionary<(Type Type, string Script), FilterScriptFunction<TResult>> m_Cache = new();

        private readonly FilterScriptFunction<TResult>? m_Default;


        public ExpressionCompiler() : this(null)
        {
        }

        public ExpressionCompiler(FilterScriptFunction<TResult>? defaultOnCompilationError)
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
                var forceDynamic = false;
                
                script = script.TrimStart();
                if(script.StartsWith("@"))
                {
                    script = script.Substring(1);
                    forceDynamic = true;
                }

                if(forceDynamic || itemType == typeof(ExpandoObject))
                {
                    function = CompileDynamic(script, itemType, log);
                }
                else
                {
                    function = CompileStatic(script, itemType, log);
                }

                m_Cache.Add(key, function);
            }

            return function;
        }        

        private FilterScriptFunction<TResult> CompileStatic(string script, Type itemType, ILog log)
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
                var parseContext = new StaticParseContext();
                PopulateParseContext(parseContext);
                parseContext.Parameters.Add(Expression.Parameter(itemType, "item"));
                parseContext.Parameters.Add(Expression.Parameter(typeof(long), "index"));
            
                var generator = new StaticCodeGenerator();
                var innerLambda = generator.CreateLambda(script, parseContext);

                var itemParameter = Expression.Parameter(typeof(object));
                var indexParameter = Expression.Parameter(typeof(long));

                // We meed to cast the item to the appropriate type expected by the
                // caller. This may fail at runtime.
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
                    log.Warn($"could not compile static function against the type {itemType.Name}. A default function will used instead");
                    return m_Default;
                }
                else
                {
                    throw new WorkbenchException($"could not compile static: {script}");
                }
            }
        }

        private FilterScriptFunction<TResult> CompileDynamic(string script, Type itemType, ILog log)
        {
            /*
             * The lambda we generate will take an IVariableReader and return "something"
             * We'll need to convert this something to the appropriate type at runtime
             */

            try
            {
                var parseContext = new DynamicParseContext();
                PopulateParseContext(parseContext);

                // We want to force certain names to always be symbols.
                // If we don't do this then "index" will be treated as System.Index!
                parseContext.AlwaysTreatAsSymbol.Add("item");
                parseContext.AlwaysTreatAsSymbol.Add("index");

                var variableReadParameter = Expression.Parameter(typeof(IVariableRead));
            
                var generator = new DynamicCodeGenerator();
                var lambda = generator.CreateLambda(script, parseContext);
                var dynamicFunction = lambda.Compile();
                
                FilterScriptFunction<TResult> scriptFunction = (item, index) =>
                {
                    var scope = new LightweightScope(CaseMode.Insensitive);
                    scope.Declare("index", index);

                    if(item is ISupportDynamic supportDynamic)
                    {
                        // This will be much easier to work with
                        scope.Declare("item", supportDynamic.MakeExpandoObject());
                    }
                    else
                    {
                        scope.Declare("item", item);
                    }

                    var result = dynamicFunction.DynamicInvoke(scope);
                    
                    // NOTE: This can throw at runtime, so beware
                    return (TResult)Convert.ChangeType(result, typeof(TResult))!;
                };

                return scriptFunction;
            }
            catch
            {
                if(m_Default is not null)
                {
                    log.Warn($"could not compile dynamic function against the type {itemType.Name}. A default function will used instead");
                    return m_Default;
                }
                else
                {
                    throw new WorkbenchException($"could not compile dynamic: {script}");
                }
            }
        }
    }
}
