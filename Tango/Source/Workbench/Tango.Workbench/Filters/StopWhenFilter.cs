﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Workbench.Filters
{
    /// <summary>
    /// Stops passing items through the pipeline when the predicate is true
    /// </summary>
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
                var predicate = GetFunction(this.Predicate, itemType);

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
