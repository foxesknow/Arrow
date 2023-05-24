using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Collections;

namespace Tango.Workbench.Sources
{
    /// <summary>
    /// Yields a sequence of items
    /// </summary>
    [Source("YieldReturn")]
    public sealed class YieldReturnSource : Source
    {
        public override IAsyncEnumerable<object> Run()
        {
            var expander = new Expander();
            var expandedItems = this.Items.Select(item => expander.Expand(item)).ToList();

            return expandedItems.ToAsyncEnumerable();
        }

        /// <summary>
        /// The items to return
        /// </summary>
        public List<string> Items{get;} = new();
    }
}
