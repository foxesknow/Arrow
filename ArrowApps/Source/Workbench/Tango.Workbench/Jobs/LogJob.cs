using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Workbench.Jobs
{
    /// <summary>
    /// Writes to the info log
    /// </summary>
    [Job("Log")]
    public sealed class LogJob : Job
    {
        public override ValueTask Run()
        {
            if(this.Message is not null) Log.Info(this.Message);
            return default;
        }

        /// <summary>
        /// The message to write
        /// </summary>
        public string? Message{get; set;}
    }
}
