using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Workbench.Filters
{
    /// <summary>
    /// Causes the pipleine to cancel after a period of time
    /// </summary>
    [Filter("CancelAfter")]
    public sealed class CancelAfterFilter : Filter
    {
        public override IAsyncEnumerable<object> Run(IAsyncEnumerable<object> items)
        {
            if(this.Delay > TimeSpan.Zero)
            {
                this.Context.CancelAfter(this.Delay);
            }

            return items;
        }

        /// <summary>
        /// The window of time over which the quantity can be returned.
        /// Once that quantity has been returned the throttle will pause for the remaining time.
        /// </summary>
        public TimeSpan Delay{get; set;}
    }
}
