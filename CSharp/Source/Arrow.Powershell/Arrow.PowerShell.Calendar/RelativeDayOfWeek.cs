using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.PowerShell.Calendar
{
	public enum RelativeDayOfWeek
	{
		FirstSunday=0,
		FirstMonday,
		FirstTuesday,
		FirstWednesday,
		FirstThursday,
		FirstFriday,
		FirstSaturday,

		SecondSunday=256,
		SecondMonday,
		SecondTuesday,
		SecondWednesday,
		SecondThursday,
		SecondFriday,
		SecondSaturday,

		ThirdSunday=512,
		ThirdMonday,
		ThirdTuesday,
		ThirdWednesday,
		ThirdThursday,
		ThirdFriday,
		ThirdSaturday,

		FourthSunday=768,
		FourthMonday,
		FourthTuesday,
		FourthWednesday,
		FourthThursday,
		FourthFriday,
		FourthSaturday,

		FifthSunday=1024,
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
			int value=((int)relativeDayOfWeek) & 0xff;
			return (DayOfWeek)value;
		}

		public static int GetWeekOffset(this RelativeDayOfWeek relativeDayOfWeek)
		{
			int value=((int)relativeDayOfWeek) >> 8;
			return value;
		}

		public static Week GetWeek(this RelativeDayOfWeek relativeDayOfWeek)
		{
			int value=((int)relativeDayOfWeek) >> 8;
			return (Week)value;
		}
	}
}
