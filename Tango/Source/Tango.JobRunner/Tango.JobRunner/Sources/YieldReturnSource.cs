using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Collections;

namespace Tango.JobRunner.Sources
{
    /// <summary>
    /// Yields a sequence of items
    /// </summary>
    [Source("YieldReturn")]
    public sealed class YieldReturnSource : Source
    {
        public override IAsyncEnumerable<object> Run()
        {
            return this.Items.ToAsyncEnumerable();
        }

        /// <summary>
        /// The items to return
        /// </summary>
        public List<string> Items{get;} = new();
    }
}
