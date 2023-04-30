using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Workbench.Filters
{
    /// <summary>
    /// Writes items to the log file and passes them through
    /// </summary>
    [Filter("Log")]
    public sealed class LogFilter : Filter
    {
        public override async IAsyncEnumerable<object> Run(IAsyncEnumerable<object> items)
        {
            await foreach(var item in items)
            {
                Log.Info(item);
                yield return item;
            }
        }
    }
}
