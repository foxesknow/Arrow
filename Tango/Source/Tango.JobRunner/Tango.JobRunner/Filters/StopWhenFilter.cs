using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.JobRunner.Filters
{
    [Filter("StopWhen")]
    public sealed class StopWhenFilter : PredicateFilterBase
    {
        public override async IAsyncEnumerable<object> Run(IAsyncEnumerable<object> items)
        {
            if(this.Predicate is null) throw new ArgumentNullException(nameof(Predicate));

            long index = 0;
            await foreach(var item in items)
            {
                var itemType = item.GetType();
                var predicate = GetPredicate(this.Predicate, itemType);

                if(predicate(item, index++)) break;

                yield return item;
            }
        }

        /// <summary>
        /// When the predicate evaluates to true we will stop returning items
        /// </summary>
        public string? Predicate{get; set;}
    }
}
