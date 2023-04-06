using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.Application.Daemon
{
    /// <summary>
    /// Hosts a deamon that does not allow the user to enter text.
    /// This is useful when the daemon is hosted in Docker
    /// </summary>
    /// <typeparam name="TDaemon"></typeparam>
    public sealed class HeadlessDaemon<TDaemon> where TDaemon : DaemonBase, new()
    {
        /// <summary>
        /// Runs the daemon
        /// </summary>
        /// <param name="args"></param>
        public void Run(string[] args)
        {
            var daemon = new TDaemon();
            daemon.StartDaemon(args);

            try
            {
                // Wait for someone to signal that we should stop
                lock(daemon.KeepRunningMonitor)
                {
                    while(daemon.KeepRunning)
                    {
                        Monitor.Wait(daemon.KeepRunningMonitor);
                    }
                }
            }
            finally
            {
                daemon.StopDaemon();
            }
        }
    }
}
