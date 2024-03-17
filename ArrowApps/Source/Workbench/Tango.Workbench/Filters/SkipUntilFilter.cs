using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Workbench.Filters
{
    /// <summary>
    /// Skips incoming items until the predicate is true.
    /// Once it is true all subsequent items will be returned.
    /// </summary>
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
                    var predicate = GetFunction(itemType);

                    if(predicate(item, index++))
                    {
                        publish = true;
                    }
                    else
                    {
                        continue;
                    }
                }

                yield return item;
            }
        }
    }
}
