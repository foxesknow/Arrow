using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Logging.Async
{
	/// <summary>
	/// Represents an item of log data that must be written to a log at some point
	/// </summary>
	public interface ILogData
	{
		/// <summary>
		/// Writes the data to the log
		/// </summary>
		void WriteToLog();
	}
}
