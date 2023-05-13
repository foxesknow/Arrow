﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Xml.ObjectCreation;
using Arrow.Application.Plugins;
using Arrow.Logging;
using Arrow.Threading.Tasks;

namespace Arrow.Application
{
    /// <summary>
    /// Encapsulates the startup and shutdown context for the application.
    /// This is split into a seperate class incase some frameworks, such as WPF
    /// don't lend themselves to the <b>ApplicationRunner</b>
    /// In this case just create an instance of the context and dispose of it at application exit.
    /// </summary>
    public class ApplicationRunContext : IAsyncDisposable
    {
        private static readonly object s_SyncRoot = new object();
        private static readonly List<Action> s_ShutdownCallbacks = new List<Action>();

        /// <summary>
        /// Starts all standard subsystems
        /// </summary>
        public ApplicationRunContext()
        {
            
        }

        public async ValueTask Start()
        {
            await PluginManager.SystemServices.Start().ContinueOnAnyContext();
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            RunRegisteredShutdownActions();
            await PluginManager.SystemServices.Stop();
            await PluginManager.SystemServices.DisposeAsync();
            
            LogManager.ShutdownLoggingSystem();
        }

        /// <summary>
        /// Register a shutdown routine that will be called when the application exits
        /// </summary>
        /// <param name="action">The action to invoke at shutdown</param>
        public static void RegisterForShutdown(Action action)
        {
            if(action == null) throw new ArgumentNullException("action");

            lock(s_SyncRoot)
            {
                s_ShutdownCallbacks.Add(action);
            }
        }

        private static void RunRegisteredShutdownActions()
        {
            lock(s_SyncRoot)
            {
                foreach(Action action in s_ShutdownCallbacks)
                {
                    try
                    {
                        action();
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}
