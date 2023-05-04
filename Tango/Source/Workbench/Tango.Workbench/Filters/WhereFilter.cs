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
    /// Applies a predicate to incoming data, only passing through the data that evaluates to true
    /// </summary>
    [Filter("Where")]
    public sealed class WhereFilter : PredicateFilterBase
    {
        public override async IAsyncEnumerable<object> Run(IAsyncEnumerable<object> items)
        {
            if(this.Predicate is null) throw new ArgumentNullException(nameof(Predicate));

            long index = 0;
            await foreach(var item in items)
            {
                var itemType = item.GetType();
                var predicate = GetFunction(itemType);

                if(predicate(item, index++)) yield return item;
            }
        }
    }
}
