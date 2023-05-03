using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Collections;

namespace Tango.Workbench.Sources
{
    [Source("Null")]
    public sealed class NullSource : Source
    {
        public override IAsyncEnumerable<object> Run()
        {
            return Enumerable.Empty<object>().ToAsyncEnumerable();
        }
    }
}
