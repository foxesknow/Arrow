using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Arrow.Application.DaemonHosting
{
    /// <summary>
    /// Hosts a deamon that does not allow the user to enter text.
    /// This is useful when the daemon is hosted in Docker
    /// </summary>
    /// <typeparam name="TDaemon"></typeparam>
    public sealed class HeadlessRunner<TDaemon> : IDaemonRunner where TDaemon : DaemonBase, new()
    {
        /// <inheritdoc/>
        public void Run(string[] args)
        {
            var daemon = new TDaemon();
            daemon.StartDaemon(args);

            RegisterPosixHandlers(daemon);

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

        private void RegisterPosixHandlers(DaemonBase daemon)
        {
#if NET6_0_OR_GREATER
            PosixSignalRegistration.Create(PosixSignal.SIGINT, context =>
            {
                if(daemon.KeepRunning)
                {
                    // It's the first time, so let the app try gracefully
                    daemon.KeepRunning = false;
                    context.Cancel = true;
                }
                else
                {
                    // We're supposed to be stopping and someone has tried again.
                    // This time we'll let the default behavior apply
                    context.Cancel = false;
                }
            });
#endif
        }
    }
}
