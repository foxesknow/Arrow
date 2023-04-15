using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

using Arrow.Logging.Loggers;
using Arrow.Xml.ObjectCreation;
using Arrow.Configuration;
using Arrow.Collections;

namespace Arrow.Logging
{
    /// <summary>
    /// Provides access to log implementations
    /// </summary>
    public static partial class LogManager
    {
        /// <summary>
        /// Raises to tell log subsystems that logging should stop (because the app is closing)
        /// </summary>
        public static event EventHandler<EventArgs>? StopLogging;

        private static readonly object s_Sync = new object();

        private static bool s_TriedToReadAppConfig;
        private static DelayedCreator? s_LogInstanceCreator;

        private static ILog? s_UnnamedLog;

        private static readonly Dictionary<string, ILog> s_Loggers = new Dictionary<string, ILog>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets the log for the caller.
        /// This method looks at the stack trace to work out the type of the object making the call
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)] // Disable inlining so that the stack trace is consistent
        public static ILog GetLog()
        {
            StackTrace stackTrace = new StackTrace(1, false);
            var frame = stackTrace.GetFrame(0);
            var method = frame?.GetMethod();
            var type = method?.DeclaringType;
            return GetLog(type);
        }

        /// <summary>
        /// Gets a named log instance
        /// </summary>
        /// <param name="name">The name of the log to get</param>
        /// <returns>A log</returns>
        public static ILog GetLog(string name)
        {
            return DoGetLog(name);
        }

        /// <summary>
        /// Gets a log for a type.
        /// </summary>
        /// <param name="type">The type that wants a log. The FullName is used to look up the log</param>
        /// <returns>A log</returns>
        public static ILog GetLog(Type? type)
        {
            var name = (type == null ? null : type.FullName);
            return DoGetLog(name);
        }

        /// <summary>
        /// Gets a log for a type
        /// </summary>
        /// <typeparam name="T">The type that wants a log. The FullName is used to look up the log</typeparam>
        /// <returns>A log</returns>
        public static ILog GetLog<T>()
        {
            var name = typeof(T).FullName;
            return DoGetLog(name);
        }

        /// <summary>
        /// Returns the default log for the system.
        /// In log4net this would be the "root" log
        /// </summary>
        /// <returns>A log</returns>
        public static ILog GetDefaultLog()
        {
            return DoGetLog(null);
        }

        /// <summary>
        /// Resolves a name to a log
        /// </summary>
        /// <param name="name">The name of the log to get</param>
        /// <returns>The log for the name</returns>
        private static ILog DoGetLog(string? name)
        {
            lock(s_Sync)
            {
                ILog? log = null;

                // If we've already got a named logger then return it
                if(name != null && s_Loggers.TryGetValue(name, out log)) return log;

                // The unnamed log is stored outside the dictionary
                if(name == null && s_UnnamedLog != null) return s_UnnamedLog;

                try
                {
                    log = CreateLogInstance();
                    var logInitializer = log as ILogInitializer;
                    if(logInitializer != null) logInitializer.Initialize(name!);
                }
                catch
                {
                    log = NullLog.Instance;
                }

                // If the log is named then store it away
                if(name != null)
                {
                    s_Loggers[name] = log;
                }
                else
                {
                    s_UnnamedLog = log;
                }

                return log;
            }
        }

        /// <summary>
        /// Returns the log to use.
        /// In the event of an error determining which log to use the null log is returned
        /// </summary>
        /// <returns>The log to use</returns>
        private static ILog CreateLogInstance()
        {
            // First, see if we need to get the log implementation from the config
            if(s_TriedToReadAppConfig == false)
            {
                try
                {
                    var node = AppConfig.GetSectionXml(ArrowSystem.Name, "Arrow.Logging/Logger");
                    if(node != null)
                    {
                        s_LogInstanceCreator = XmlCreation.DelayedCreate<ILog>(node);
                    }
                }
                catch
                {
                    // Ignore anything that goes wrong
                }

                s_TriedToReadAppConfig = true;
            }

            // If we get here and the default logger is null then something must
            // have gone wrong when trying to create an instance
            if(s_LogInstanceCreator == null) return NullLog.Instance;

            ILog? log = null;

            try
            {
                log = s_LogInstanceCreator.Create<ILog>();
            }
            catch
            {
                s_LogInstanceCreator = null;
                log = NullLog.Instance;
            }

            return log;
        }

        /// <summary>
        /// This is called by the Application namespace
        /// to allow any logging subsystems to shutdown gracefully
        /// </summary>
        internal static void ShutdownLoggingSystem()
        {
            var d = StopLogging;
            if(d != null) d(null, EventArgs.Empty);
        }
    }
}
