using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Workbench.Filters
{
    /// <summary>
    /// Calls ToString() on each item in the sequence.
    /// If the item returns null as its string representation then the item is not passed through.
    /// </summary>
    [Filter("ToString")]
    public sealed class ToStringFilter : Filter
    {
        public override async IAsyncEnumerable<object> Run(IAsyncEnumerable<object> items)
        {
            await foreach(var item in items)
            {
                var asString = item.ToString();
                if(asString is not null) yield return asString;
            }
        }
    }
}
