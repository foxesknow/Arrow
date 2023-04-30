using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Arrow.Scripting.Wire;
using Arrow.Scripting.Wire.StaticExpression;

namespace Tango.Workbench.Filters
{
    /// <summary>
    /// Base class for all filters that preform some sort of filtering.
    /// 
    /// The predicate that is compiles has an "item" and "index" parameter.
    /// "item" is the item going through the pipeline
    /// "index" is the 0-based position of the item in the stream
    /// </summary>
    public abstract class PredicateFilterBase : Filter
    {
        private static Func<object, long, bool> AlwaysFalse = static (_, _) => false;

        private readonly Dictionary<Type, Func<object, long, bool>> m_Cache = new();

        /// <summary>
        /// Fetches the predicate for the given type, compiling the script on demand if required.
        /// It the scripts fails to compile for any reason a predicate that evaluates to false will be returned
        /// </summary>
        /// <param name="script"></param>
        /// <param name="itemType"></param>
        /// <returns></returns>
        protected Func<object, long, bool> GetPredicate(string script, Type itemType)
        {
            if(m_Cache.TryGetValue(itemType, out var predicate) == false)
            {
                predicate = Compile(script, itemType);
                m_Cache.Add(itemType, predicate);
            }

            return predicate;
        }

        private Func<object, long, bool> Compile(string script, Type itemType)
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
                
                var outerLambda = Expression.Lambda<Func<object, long, bool>>
                (
                    Expression.Invoke
                    (
                        innerLambda,
                        Expression.Convert(itemParameter, itemType),
                        indexParameter
                    ),
                    itemParameter,
                    indexParameter
                );

                return outerLambda.Compile();
            }
            catch
            {
                Log.Warn($"could not compile the predicate against the type {itemType.Name}. A default predicate will used that evaluates to false instead");

                return AlwaysFalse;
            }
        }

        private StaticParseContext MakeParseContext()
        {
            var parseContext = new StaticParseContext();
            parseContext.References.Add(typeof(string).Assembly);
            parseContext.Usings.Add("System");

            parseContext.References.Add(typeof(System.IO.Path).Assembly);
            parseContext.Usings.Add("System.IO");

            parseContext.References.Add(typeof(Arrow.Calendar.Clock).Assembly);
            parseContext.Usings.Add("Arrow.Calendar");

            return parseContext;
        }
    }
}
