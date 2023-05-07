using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Logging.Loggers
{
    /// <summary>
    /// Base class for loggers that write to the console
    /// </summary>
    public abstract class ConsoleLog : TextWriterLog
    {
        private readonly bool m_Colorize;

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="output"></param>
        /// <param name="colorize"></param>
        /// <param name="syncRoot"></param>
        protected ConsoleLog(TextWriter output, bool colorize, object syncRoot) : base(output, syncRoot)
        {
            m_Colorize = colorize;
        }

        /// <summary>
        /// Write to the console, using color is requested
        /// </summary>
        /// <param name="consoleLevel"></param>
        /// <param name="line"></param>
        protected override sealed void WriteLine(in LoggingInfo consoleLevel, object line)
        {
            if(m_Colorize)
            {
                lock(this.SyncRoot)
                {
                    using(new ColorChange(consoleLevel.Color, m_Colorize))
                    {
                        this.Output.WriteLine(line);
                    }
                }
            }
            else
            {
                base.WriteLine(in consoleLevel, line);
            }
        }
    }
}
