using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.PowerShell.Calendar
{
	static class DateTimeExtensions
	{
		/// <summary>
		/// Returns the first day of the dayOfWeek in the month
		/// </summary>
		/// <param name="date">The date to use</param>
		/// <param name="dayOfWeek">The day to get</param>
		/// <returns>A date time</returns>
		public static DateTime GetFirstDayOfWeek(this DateTime date, DayOfWeek dayOfWeek)
		{
			DateTime baseline=GetFirstDayOfMonth(date);

			while(baseline.DayOfWeek!=dayOfWeek)
			{
				baseline=baseline.AddDays(1);
			}

			return baseline;
		}

		
		/// <summary>
		/// Returns the last day of the dayOfWeek in the month
		/// </summary>
		/// <param name="date">The date to use</param>
		/// <param name="dayOfWeek">The day to get</param>
		/// <returns>A date time</returns>
		public static DateTime GetLastDayOfWeek(this DateTime date, DayOfWeek dayOfWeek)
		{
			// First, work out the last day.
			// The easiest way to do this is get the first day of the next month and subtract a day

			var baseline=GetFirstDayOfMonth(date);
			baseline=baseline.AddMonths(1).AddDays(-1);

			while(baseline.DayOfWeek!=dayOfWeek)
			{
				baseline=baseline.AddDays(-1);
			}

			return baseline;
		}

		


		private static DateTime GetFirstDayOfMonth(DateTime date)
		{
			DateTime baseline=new DateTime(date.Year,date.Month,1,date.Hour,date.Minute,date.Second,date.Kind);
			return baseline;
		}
	}
}
