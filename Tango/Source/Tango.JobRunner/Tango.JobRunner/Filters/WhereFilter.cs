using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using Arrow.Scripting.Wire;
using Arrow.Scripting.Wire.StaticExpression;

namespace Tango.JobRunner.Filters
{
    /// <summary>
    /// Applies a predicate to incoming data, only passing through the data that evaluates to true
    /// </summary>
    [Filter("Where")]
    public sealed class WhereFilter : Filter
    {
        private static Func<object, bool> AlwaysFalse = static _ => false;

        public override async IAsyncEnumerable<object> Run(IAsyncEnumerable<object> items)
        {
            if(this.Predicate is null) throw new ArgumentNullException(nameof(Predicate));

            Dictionary<Type, Func<object, bool>> predicates = new();

            await foreach(var item in items)
            {
                var itemType = item.GetType();

                if(predicates.TryGetValue(itemType, out var predicate) == false)
                {
                    predicate = Compile(this.Predicate, itemType);
                    predicates.Add(itemType, predicate);
                }

                if(predicate(item)) yield return item;
            }
        }

        private Func<object, bool> Compile(string script, Type itemType)
        {
            try
            {
                var parseContext = new StaticParseContext();
                parseContext.Parameters.Add(Expression.Parameter(itemType, "item"));
            
                var generator = new StaticCodeGenerator();
                var innerLambda = generator.CreateLambda(script, parseContext);

                var itemParameter = Expression.Parameter(typeof(object));
                var outerLambda = Expression.Lambda<Func<object, bool>>
                (
                    Expression.Invoke
                    (
                        innerLambda,
                        Expression.Convert(itemParameter, itemType)
                    ),
                    itemParameter
                );

                return outerLambda.Compile();
            }
            catch
            {
                Log.Warn($"could not compile the predicate against the type {itemType.Name}. A default predicate will used that evaluates to false instead");

                return AlwaysFalse;
            }
        }

        /// <summary>
        /// The predicate to apply to each item
        /// </summary>
        public string? Predicate{get; set;}
    }
}
