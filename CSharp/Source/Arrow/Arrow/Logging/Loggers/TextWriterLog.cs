using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Logging.Loggers
{
    /// <summary>
    /// A logger that writers to a TextWriter instance.
    /// </summary>
    public class TextWriterLog : BaseLog
    {
        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="output"></param>
        public TextWriterLog(TextWriter output): this(output, new())
        {
        }

        /// <summary>
        /// Initializes the instances, using a sync root provided by a derived class
        /// </summary>
        /// <param name="output"></param>
        /// <param name="syncRoot"></param>
        /// <exception cref="ArgumentNullException"></exception>
        protected TextWriterLog(TextWriter output, object syncRoot)
        {
            if(output is null) throw new ArgumentNullException(nameof(output));
            if(syncRoot is null) throw new ArgumentNullException(nameof(syncRoot));

            this.Output = output;
            this.SyncRoot = syncRoot;
        }

        /// <summary>
        /// Where to write to
        /// </summary>
        protected TextWriter Output{get;}

        /// <summary>
        /// The sync root to be used to serialize writing to the TextWriter
        /// </summary>
        protected object SyncRoot{get;}

        /// <summary>
        /// Writes a line to the output
        /// </summary>
        /// <param name="consoleLevel"></param>
        /// <param name="line"></param>
        protected override void WriteLine(in LoggingInfo consoleLevel, object line)
        {
            lock(this.SyncRoot)
            {
                this.Output.WriteLine(line);
            }
        }
    }
}
