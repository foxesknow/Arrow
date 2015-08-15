using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.PowerShell.Calendar
{
	public enum RelativeDayOfWeek
	{
		FirstSunday,
		FirstMonday,
		FirstTuesday,
		FirstWednesday,
		FirstThursday,
		FirstFriday,
		FirstSaturday,

		SecondSunday,
		SecondMonday,
		SecondTuesday,
		SecondWednesday,
		SecondThursday,
		SecondFriday,
		SecondSaturday,

		ThirdSunday,
		ThirdMonday,
		ThirdTuesday,
		ThirdWednesday,
		ThirdThursday,
		ThirdFriday,
		ThirdSaturday,

		FourthSunday,
		FourthMonday,
		FourthTuesday,
		FourthWednesday,
		FourthThursday,
		FourthFriday,
		FourthSaturday,

		FifthSunday,
		FifthMonday,
		FifthTuesday,
		FifthWednesday,
		FifthThursday,
		FifthFriday,
		FifthSaturday,
	}

	static class RelativeDayOfWeekExtensions
	{
		public static DayOfWeek GetDayOfWeek(this RelativeDayOfWeek relativeDayOfWeek)
		{
			switch(relativeDayOfWeek)
			{
				case RelativeDayOfWeek.FirstSunday:
				case RelativeDayOfWeek.SecondSunday:
				case RelativeDayOfWeek.ThirdSunday:
				case RelativeDayOfWeek.FourthSunday:
				case RelativeDayOfWeek.FifthSunday:
					return DayOfWeek.Sunday;

				case RelativeDayOfWeek.FirstMonday:
				case RelativeDayOfWeek.SecondMonday:
				case RelativeDayOfWeek.ThirdMonday:
				case RelativeDayOfWeek.FourthMonday:
				case RelativeDayOfWeek.FifthMonday:
					return DayOfWeek.Monday;

				case RelativeDayOfWeek.FirstTuesday:
				case RelativeDayOfWeek.SecondTuesday:
				case RelativeDayOfWeek.ThirdTuesday:
				case RelativeDayOfWeek.FourthTuesday:
				case RelativeDayOfWeek.FifthTuesday:
					return DayOfWeek.Tuesday;

				case RelativeDayOfWeek.FirstWednesday:
				case RelativeDayOfWeek.SecondWednesday:
				case RelativeDayOfWeek.ThirdWednesday:
				case RelativeDayOfWeek.FourthWednesday:
				case RelativeDayOfWeek.FifthWednesday:
					return DayOfWeek.Wednesday;

				case RelativeDayOfWeek.FirstThursday:
				case RelativeDayOfWeek.SecondThursday:
				case RelativeDayOfWeek.ThirdThursday:
				case RelativeDayOfWeek.FourthThursday:
				case RelativeDayOfWeek.FifthThursday:
					return DayOfWeek.Thursday;

				case RelativeDayOfWeek.FirstFriday:
				case RelativeDayOfWeek.SecondFriday:
				case RelativeDayOfWeek.ThirdFriday:
				case RelativeDayOfWeek.FourthFriday:
				case RelativeDayOfWeek.FifthFriday:
					return DayOfWeek.Friday;

				case RelativeDayOfWeek.FirstSaturday:
				case RelativeDayOfWeek.SecondSaturday:
				case RelativeDayOfWeek.ThirdSaturday:
				case RelativeDayOfWeek.FourthSaturday:
				case RelativeDayOfWeek.FifthSaturday:
					return DayOfWeek.Saturday;

				default:
					throw new ArgumentException("Unexpected relative day of week: "+relativeDayOfWeek);
			}
		}

		public static int GetWeekOffset(this RelativeDayOfWeek relativeDayOfWeek)
		{
			switch(relativeDayOfWeek)
			{
				case RelativeDayOfWeek.FirstSunday:
				case RelativeDayOfWeek.FirstMonday:
				case RelativeDayOfWeek.FirstTuesday:
				case RelativeDayOfWeek.FirstWednesday:
				case RelativeDayOfWeek.FirstThursday:
				case RelativeDayOfWeek.FirstFriday:
				case RelativeDayOfWeek.FirstSaturday:
					return 0;

				case RelativeDayOfWeek.SecondSunday:
				case RelativeDayOfWeek.SecondMonday:
				case RelativeDayOfWeek.SecondTuesday:
				case RelativeDayOfWeek.SecondWednesday:
				case RelativeDayOfWeek.SecondThursday:
				case RelativeDayOfWeek.SecondFriday:
				case RelativeDayOfWeek.SecondSaturday:
					return 1;

				case RelativeDayOfWeek.ThirdSunday:
				case RelativeDayOfWeek.ThirdMonday:
				case RelativeDayOfWeek.ThirdTuesday:
				case RelativeDayOfWeek.ThirdWednesday:
				case RelativeDayOfWeek.ThirdThursday:
				case RelativeDayOfWeek.ThirdFriday:
				case RelativeDayOfWeek.ThirdSaturday:
					return 2;

				case RelativeDayOfWeek.FourthSunday:
				case RelativeDayOfWeek.FourthMonday:
				case RelativeDayOfWeek.FourthTuesday:
				case RelativeDayOfWeek.FourthWednesday:
				case RelativeDayOfWeek.FourthThursday:
				case RelativeDayOfWeek.FourthFriday:
				case RelativeDayOfWeek.FourthSaturday:
					return 3;

				case RelativeDayOfWeek.FifthSunday:
				case RelativeDayOfWeek.FifthMonday:
				case RelativeDayOfWeek.FifthTuesday:
				case RelativeDayOfWeek.FifthWednesday:
				case RelativeDayOfWeek.FifthThursday:
				case RelativeDayOfWeek.FifthFriday:
				case RelativeDayOfWeek.FifthSaturday:
					return 4;

				default:
					throw new ArgumentException("Unexpected relative day of week: "+relativeDayOfWeek);
			}
		}
	}
}
