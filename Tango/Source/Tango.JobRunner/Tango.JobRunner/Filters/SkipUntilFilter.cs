using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.JobRunner.Filters
{
    [Filter("SkipUntil")]
    public sealed class SkipUntilFilter : PredicateFilterBase
    {
        public override async IAsyncEnumerable<object> Run(IAsyncEnumerable<object> items)
        {
            if(this.Predicate is null) throw new ArgumentNullException(nameof(Predicate));

            long index = 0;
            bool publish = false;
            
            await foreach(var item in items)
            {
                if(publish == false)
                {
                    var itemType = item.GetType();
                    var predicate = GetPredicate(this.Predicate, itemType);

                    if(predicate(item, index++) == false)
                    {
                        continue;
                    }
                    else
                    {
                        publish = true;
                    }
                }

                yield return item;
            }
        }

        /// <summary>
        /// While the predicate is true we start returning items
        /// </summary>
        public string? Predicate{get; set;}
    }
}
