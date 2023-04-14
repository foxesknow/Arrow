using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Calendar
{
	/// <summary>
	/// Returns the current date/time
	/// </summary>
	public class SystemClockDriver : IClockDriver
	{
		/// <summary>
		/// Returns DateTime.Now
		/// </summary>
		public DateTime Now
		{
			get{return DateTime.Now;}
		}

		/// <summary>
		/// Returns DateTime.UtcNow
		/// </summary>
		public DateTime UtcNow
		{
			get{return DateTime.UtcNow;}
		}
	}
}
