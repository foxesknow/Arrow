using Arrow.Logging.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Logging
{
    /// <summary>
	/// Writes to the console window.
	/// Output is sent to Console.Out so that it can be captured by redirection
	/// </summary>
    public sealed class StdoutLog : BaseConsoleLog
    {
        private static readonly object s_Sync=new object();
		
		public StdoutLog() : base(Console.Error, Console.IsErrorRedirected, s_Sync)
		{
		}
    }
}
