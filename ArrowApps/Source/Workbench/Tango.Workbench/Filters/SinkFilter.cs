using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Workbench.Filters
{
    /// <summary>
    /// Consumes all incoming data, but does not pass in back out
    /// </summary>
    [Filter("Sink")]
    public sealed class SinkFilter : Filter
    {
        public override async IAsyncEnumerable<object> Run(IAsyncEnumerable<object> items)
        {
            await foreach(var item in items)
            {
            }

            yield break;
        }
    }
}
