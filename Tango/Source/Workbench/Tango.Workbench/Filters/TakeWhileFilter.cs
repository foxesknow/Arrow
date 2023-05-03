using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Workbench.Filters
{
    [Filter("TakeWhile")]
    public sealed class TakeWhileFilter : PredicateFilterBase
    {
        public override async IAsyncEnumerable<object> Run(IAsyncEnumerable<object> items)
        {
            if(this.Predicate is null) throw new ArgumentNullException(nameof(Predicate));

            long index = 0;
            
            await foreach(var item in items)
            {
                var itemType = item.GetType();
                var predicate = GetPredicate(this.Predicate, itemType);

                if(predicate(item, index++))
                {
                    yield return item;
                }
                else
                {
                    yield break;
                }
            }
        }

        /// <summary>
        /// While the predicate is true we will return items
        /// </summary>
        public string? Predicate{get; set;}
    }
}
