using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Arrow.Execution;

namespace Arrow.Logging.Loggers
{
	/// <summary>
	/// Writes to the console window.
	/// Output is sent to Console.Error so that it will not mix with redirected output
	/// </summary>
	public sealed class StderrLog : ConsoleLog
	{
		private static readonly object s_Sync = new object();
		
		public StderrLog() : base(Console.Error, !Console.IsErrorRedirected, s_Sync)
		{
		}
	}
}
