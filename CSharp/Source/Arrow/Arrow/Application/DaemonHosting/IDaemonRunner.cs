using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Application.DaemonHosting
{
    public interface IDaemonRunner
    {
        /// <summary>
        /// Runs the daemon.
        /// This method will only return when the daemon shuts down
        /// </summary>
        /// <param name="args"></param>
        public void Run(string[] args);
    }
}
