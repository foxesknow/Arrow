using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Workbench.Filters
{
    /// <summary>
    /// Takes item from the stream until the predicates is true
    /// </summary>
    [Filter("TakeUntil")]
    public sealed class TakeUntilFilter : PredicateFilterBase
    {
        public override async IAsyncEnumerable<object> Run(IAsyncEnumerable<object> items)
        {
            if(this.Predicate is null) throw new ArgumentNullException(nameof(Predicate));

            long index = 0;
            
            await foreach(var item in items)
            {
                var itemType = item.GetType();
                var predicate = GetFunction(itemType);

                if(predicate(item, index++))
                {
                    yield break;
                }
                else
                {
                    yield return item;
                }
            }
        }
    }
}
