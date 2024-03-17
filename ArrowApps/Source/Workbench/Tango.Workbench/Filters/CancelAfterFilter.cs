using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Workbench.Filters
{
    /// <summary>
    /// Causes the filter sequence to be cancel after a period of time
    /// </summary>
    [Filter("CancelAfter")]
    public sealed class CancelAfterFilter : Filter
    {
        public override IAsyncEnumerable<object> Run(IAsyncEnumerable<object> items)
        {
            if(this.Delay > TimeSpan.Zero)
            {
                this.Context.CancelAfter(this.Delay);
                VerboseLog.Info($"Sequence will be cancelled in {Delay}");
            }

            return items;
        }

        /// <summary>
        /// Cancels the filter.
        /// If the tee filter is part of a "tee" then the cancel may cancel either the tee
        /// sequence or the overall group, depending on how the Tee is configured.
        /// </summary>
        public TimeSpan Delay{get; set;}
    }
}
