﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Application.Daemon
{
    /// <summary>
    /// Hosts a daemon that can read from the console
    /// </summary>
    public class ConsoleDaemon<TDaemon> where TDaemon : DaemonBase, new()
    {
        /// <summary>
        /// Runs the daemon
        /// </summary>
        /// <param name="args">And command line arguments to the application</param>
        public void Run(string[] args)
        {
            var daemon = new TDaemon();
            daemon.StartDaemon(args);

            try
            {
            }
            finally
            {
                daemon.StopDaemon();
            }
        }

        /// <summary>
        /// Waits for an external signal to indicate the daemon should shutdown.
        /// By default this comes from the user typing "exit" in the console.
        /// </summary>
        /// <param name="daemon">The daemon that is running</param>
        /// <param name="args">And command line arguments passed to the application</param>
        protected virtual void WaitForShutdown(TDaemon daemon, string[] args)
        {
            while(daemon.KeepRunning)
            {
                Console.Write(daemon.CommandLinePrompt);
                var line = Console.ReadLine()!.Trim();

                if(string.IsNullOrWhiteSpace(line) == false)
                {
                    ProcessUserInput(daemon, line);
                }
            }
        }

        private void ProcessUserInput(DaemonBase daemon, string line)
        {
            daemon.ProcessUserInput(line);
        }
    }
}
