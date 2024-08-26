﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Workbench.Filters
{
    /// <summary>
    /// Skips items whilst the predicate is true. 
    /// Once it goes to false all subsequent items will be returned.
    /// </summary>
    [Filter("SkipWhile")]
    public sealed class SkipWhileFilter : PredicateFilterBase
    {
        public override async IAsyncEnumerable<object> Run(IAsyncEnumerable<object> items)
        {
            if(this.Predicate is null) throw new ArgumentNullException(nameof(Predicate));

            long index = 0;
            bool keepSkipping = true;
            
            await foreach(var item in items)
            {
                if(keepSkipping)
                {
                    var itemType = item.GetType();
                    var predicate = GetFunction(itemType);

                    if(predicate(item, index++))
                    {
                        continue;
                    }
                    else
                    {
                        keepSkipping = false;
                    }
                }

                yield return item;
            }
        }
    }
}