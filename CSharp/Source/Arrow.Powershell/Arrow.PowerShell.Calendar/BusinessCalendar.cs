using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.PowerShell.Calendar
{
	public class BusinessCalendar : ISupportInitialize
	{
		private readonly HashSet<DayOfWeek> m_WeekendDays=new HashSet<DayOfWeek>();
		private readonly HashSet<BusinessDate> m_Holidays=new HashSet<BusinessDate>();
		private readonly HashSet<BusinessDate> m_RecuringHolidays=new HashSet<BusinessDate>();

		public ISet<DayOfWeek> Weekends
		{
			get{return m_WeekendDays;}
		}

		public ISet<BusinessDate> Holidays
		{
			get{return m_Holidays;}
		}

		public ISet<BusinessDate> RecuringHolidays
		{
			get{return m_RecuringHolidays;}
		}

		public bool IsWeekend(DayOfWeek dayOfWeek)
		{
			return m_WeekendDays.Contains(dayOfWeek);
		}

		public bool IsWeekend(DateTime date)
		{
			return m_WeekendDays.Contains(date.DayOfWeek);
		}

		public bool IsHoliday(DateTime date)
		{
			var businessDate=ToBusinessDate(date);
			if(m_Holidays.Contains(businessDate)) return true;

			// The recuring ones are a bit more involved
			// We just need to compare the month and day, not the year
			var match=m_RecuringHolidays.FirstOrDefault(b=>b.Month==businessDate.Month && b.Day==businessDate.Day);

			return match!=null;
		}

		public bool IsBusinessDate(DateTime date)
		{
			if(IsWeekend(date.DayOfWeek)) return false;
			if(IsHoliday(date)) return false;

			return true;
		}

		private BusinessDate ToBusinessDate(DateTime date)
		{
			return new BusinessDate(date.Year,date.Month,date.Day);
		}

		void ISupportInitialize.BeginInit()
		{
			// No need to do anything
		}

		void ISupportInitialize.EndInit()
		{
			// If we've got 7 weekend days then something is wrong!
			if(m_WeekendDays.Count==7) throw new InvalidOperationException("can't have 7 weekend days!");
		}

	}
}
