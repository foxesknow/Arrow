using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Workbench.Filters
{
    [Filter("Null")]
    public sealed class NullFilter : Filter
    {
        public override IAsyncEnumerable<object> Run(IAsyncEnumerable<object> items)
        {
            return items;
        }
    }
}
