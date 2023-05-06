using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Logging.Loggers
{
    /// <summary>
    /// Provides information on a logging level to allow implementation to work out how to render a line
    /// </summary>
    public readonly struct LoggingInfo
    {
        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="prefix"></param>
        /// <param name="color"></param>
        public LoggingInfo(LogLevel logLevel, string prefix, ConsoleColor color)
        {
            this.LogLevel = logLevel;
            this.Prefix = prefix;
            this.Color = color;
        }

        /// <summary>
        /// The logging level
        /// </summary>
        public LogLevel LogLevel{get;}

        /// <summary>
        /// The color it should be rendered, if appropriate
        /// </summary>
        public ConsoleColor Color{get;}

        /// <summary>
        /// The prefix to apply to a line, if appropriate
        /// </summary>
        public string Prefix{get;}

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.LogLevel.ToString();
        }
    }
}
