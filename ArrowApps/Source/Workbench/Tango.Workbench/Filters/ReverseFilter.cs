using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Workbench.Filters
{
    /// <summary>
    /// Reverses the incoming data.
    /// NOTE: To do this all the incoming data must be consumed before it is passed through
    /// </summary>
    [Filter("Reverse")]
    public sealed class ReverseFilter : Filter
    {
        public async override IAsyncEnumerable<object> Run(IAsyncEnumerable<object> items)
        {
            var incomingData = new List<object>();

            await foreach(var item in items)
            {
                incomingData.Add(item);
            }

            for(var i = incomingData.Count - 1; i >= 0; i--)
            {
                yield return incomingData[i];
            }
        }
    }
}
