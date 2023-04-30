using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Logging;
using Arrow.Logging.Loggers;

namespace Tango.Workbench
{
    /// <summary>
    /// Base class for all jobs
    /// </summary>
    public abstract class Job : Runnable
    {
        /// <summary>
        /// Runs the actual job
        /// </summary>
        /// <returns></returns>
        public abstract ValueTask Run();        
    }
}
