using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micron
{
    [Flags]
    public enum DaysOfTheWeek
    {
        None = 0,
        
        Sun = 1,
        Sunday = 1,

        Mon = 2,
        Monday = 2,

        Tue = 4,
        Tuesday = 4,

        Wed = 8,
        Wednesday = 8,

        Thu = 16,
        Thursday = 16,

        Fri = 32,
        Friday = 32,

        Sat = 64,
        Saturday = 64,

        Weekdays = Mon | Tue | Wed | Thu | Fri,
        Weekends = Sat | Sun
    }

    public static class DaysOfTheWeekExtensions
    {
        public static IReadOnlyList<DayOfWeek> ToDayOfWeekList(this DaysOfTheWeek daysOfTheWeek)
        {
            if(daysOfTheWeek == DaysOfTheWeek.None) return Array.Empty<DayOfWeek>();

            var days = new List<DayOfWeek>(7);

            if((daysOfTheWeek & DaysOfTheWeek.Sun) != 0) days.Add(DayOfWeek.Sunday);
            if((daysOfTheWeek & DaysOfTheWeek.Mon) != 0) days.Add(DayOfWeek.Monday);
            if((daysOfTheWeek & DaysOfTheWeek.Tue) != 0) days.Add(DayOfWeek.Tuesday);
            if((daysOfTheWeek & DaysOfTheWeek.Wed) != 0) days.Add(DayOfWeek.Wednesday);
            if((daysOfTheWeek & DaysOfTheWeek.Thu) != 0) days.Add(DayOfWeek.Thursday);
            if((daysOfTheWeek & DaysOfTheWeek.Fri) != 0) days.Add(DayOfWeek.Friday);
            if((daysOfTheWeek & DaysOfTheWeek.Sat) != 0) days.Add(DayOfWeek.Saturday);

            return days;
        }
    }
}
