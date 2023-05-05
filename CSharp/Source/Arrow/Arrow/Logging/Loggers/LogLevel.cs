using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Logging.Loggers
{
    [Flags]
    public enum LogLevel
    {
        /// <summary>
        /// No logging
        /// </summary>
        None = 0,

        /// <summary>
        /// Debug logging
        /// </summary>
        Debug = 1,

        /// <summary>
        /// Info logging
        /// </summary>
        Info = 2,

        /// <summary>
        /// Warning logging
        /// </summary>
        Warn = 4,

        /// <summary>
        /// Error logging
        /// </summary>
        Error = 8,

        /// <summary>
        /// Fatal logging
        /// </summary>
        Fatal = 16,

        /// <summary>
        /// Log everything
        /// </summary>
        All = Debug | Info | Warn | Error | Fatal,
        
        /// <summary>
        /// Log everything except debug
        /// </summary>
        NotDebug = Info | Warn | Error | Fatal,

        /// <summary>
        /// Log debug and above
        /// </summary>
        DebugAndAbove = All,

        /// <summary>
        /// Log info and above
        /// </summary>
        InfoAndAbove = Info | Warn | Error | Fatal,

        /// <summary>
        /// Log warning and above
        /// </summary>
        WarnAndAbove = Warn | Error | Fatal,

        /// <summary>
        /// Log error and above
        /// </summary>
        ErrorAndAbove = Error | Fatal,
    }
}
