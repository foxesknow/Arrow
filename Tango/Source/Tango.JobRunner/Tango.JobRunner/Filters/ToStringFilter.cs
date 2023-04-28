using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.JobRunner.Filters
{
    /// <summary>
    /// Calls ToString() on each item in the sequence.
    /// If the call returns null then an empty string is returned.
    /// </summary>
    [Filter("ToString")]
    public sealed class ToStringFilter : Filter
    {
        public override async IAsyncEnumerable<object> Run(IAsyncEnumerable<object> items)
        {
            await foreach(var item in items)
            {
                yield return item.ToString() ?? "";
            }
        }
    }
}
