using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Workbench.Jobs
{
    /// <summary>
    /// Triggers a cancel in a group after a given amount of time
    /// </summary>
    public sealed class CancelAfterJob : Job
    {
        public override ValueTask Run()
        {
            if(this.Delay > TimeSpan.Zero)
            {
                this.Context.CancelAfter(this.Delay);
                VerboseLog.Info($"Group will be cancelled in {Delay}");
            }

            return default;
        }

        /// <summary>
        /// Cancels the job, and group, after the specified delay
        /// </summary>
        public TimeSpan Delay{get; set;}
    }
}
