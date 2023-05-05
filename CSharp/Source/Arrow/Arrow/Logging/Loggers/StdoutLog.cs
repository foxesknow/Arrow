using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Logging.Loggers
{
    /// <summary>
	/// Writes to the console window.
	/// Output is sent to Console.Out so that it can be captured by redirection
	/// </summary>
    public sealed class StdoutLog : BaseConsoleLog
    {
        private static readonly object s_Sync = new object();

        /// <summary>
        /// Initializes the instance
        /// </summary>
        public StdoutLog() : base(Console.Out, Console.IsOutputRedirected, s_Sync)
        {
        }
    }
}
