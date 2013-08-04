using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Execution
{
	/// <summary>
	/// How to measure execution time
	/// </summary>
	public enum TimerMode
	{
		/// <summary>
		/// Measure in milliseconds
		/// </summary>
		Milliseconds,
		
		/// <summary>
		/// Measure in microseconds
		/// </summary>
		Microseconds,
		
		/// <summary>
		/// Measure in nanoseconds
		/// </summary>
		Nanoseconds
	}
}
