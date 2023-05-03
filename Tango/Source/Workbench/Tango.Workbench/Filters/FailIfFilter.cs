using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Workbench.Filters
{
    /// <summary>
    /// A filter that will throw an exception if a predicate evaluates to true.
    /// </summary>
    [Filter("FailIf")]
    public sealed class FailIfFilter : PredicateFilterBase
    {
        public override async IAsyncEnumerable<object> Run(IAsyncEnumerable<object> items)
        {
            if(this.Predicate is null) throw new ArgumentNullException(nameof(Predicate));

            long index = 0;

            await foreach(var item in items)
            {
                var itemType = item.GetType();
                var predicate = GetFunction(this.Predicate, itemType);

                if(predicate(item, index++))
                {
                    throw new WorkbenchException(this.Message);
                }

                yield return item;
            }
        }

        /// <summary>
        /// If this predicate is true we will throw an exception
        /// </summary>
        public string? Predicate{get; set;}

        /// <summary>
        /// The message to place in the exception
        /// </summary>
        public string? Message{get; set;}
    }
}
