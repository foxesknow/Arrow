using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Application.DaemonHosting
{
    /// <summary>
    /// Base class for all daemon style applications
    /// </summary>
    public abstract partial class DaemonBase
    {
        /// <summary>
        /// Starts the daemon.
        /// This method must return once it was started te daemon 
        /// so any command line proceesing can take place
        /// </summary>
        /// <param name="args"></param>
        protected internal abstract ValueTask StartDaemon(string[] args);
        
        /// <summary>
        /// Called to stop the daemon
        /// </summary>
        protected internal abstract ValueTask StopDaemon();
    }
}
