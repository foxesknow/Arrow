using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using Arrow.Scripting.Wire.StaticExpression;

namespace Tango.Workbench.Filters
{
    [Filter("Map")]
    public sealed class MapFilter : ExpressionFilterBase<object?>
    {
        private static readonly Func<object, long, object?> AlwaysNull = static (_, _) => null;

        public override async IAsyncEnumerable<object> Run(IAsyncEnumerable<object> items)
        {
            if(this.Transformation is null) throw new ArgumentNullException(nameof(Transformation));

            long index = 0;

            await foreach(var item in items)
            {
                var itemType = item.GetType();
                var function = GetFunction(this.Transformation, itemType);

                var newItem = function(item, index++);
                if(newItem is not null) yield return newItem;
            }
        }

        protected override Func<object, long, object?> Compile(string script, Type itemType)
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
                
                var outerLambda = Expression.Lambda<Func<object, long, object?>>
                (
                    Expression.Convert
                    (
                        Expression.Invoke
                        (
                            innerLambda,
                            Expression.Convert(itemParameter, itemType),
                            indexParameter
                        ),
                        typeof(object)
                    ),
                    itemParameter,
                    indexParameter
                );

                return outerLambda.Compile();
            }
            catch
            {
                Log.Warn($"could not compile the function against the type {itemType.Name}. A default predicate will used that evaluates to null instead");

                return AlwaysNull;
            }
        }

        /// <summary>
        /// The transformation that will be applied
        /// </summary>
        public string? Transformation{get; set;}
    }
}
