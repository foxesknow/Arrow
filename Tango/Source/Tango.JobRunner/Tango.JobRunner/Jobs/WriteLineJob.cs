using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.JobRunner.Jobs
{
    /// <summary>
    /// Writes a line to the console
    /// </summary>
    [Job("WriteLine")]
    public sealed class WriteLineJob : Job
    {
        public override async ValueTask Run()
        {
            if(this.Message is not null)
            {
                var writer = (this.AsError ? Console.Error : Console.Out);

                await writer.WriteLineAsync(this.Message);
            }
        }

        /// <summary>
        /// The message to write
        /// </summary>
        public string? Message{get; set;}

        /// <summary>
        /// True to write to stderr, false to write to stdout
        /// </summary>
        public bool AsError{get; set;}
    }
}
