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

        protected ConsoleLog(TextWriter output, bool colorize, object syncRoot) : base(output, syncRoot)
        {
            m_Colorize = colorize;
        }

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
