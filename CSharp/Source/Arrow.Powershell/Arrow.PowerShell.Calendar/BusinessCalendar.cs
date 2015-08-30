using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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

		public XmlDocument ToXml()
		{
			var document=new XmlDocument();

			var root=document.CreateElement("BusinessCalendar");
			document.AppendChild(root);

			var weekends=document.CreateElement("Weekends");
			root.AppendChild(weekends);
			foreach(var day in AsSorted(this.Weekends))
			{
				var node=document.CreateElement("Day");
				node.InnerText=day.ToString();

				weekends.AppendChild(node);
			}

			var holidays=document.CreateElement("Holidays");
			root.AppendChild(holidays);
			foreach(var holiday in AsSorted(this.Holidays))
			{
				var node=document.CreateElement("Date");
				node.InnerText=holiday.ToString();

				holidays.AppendChild(node);
			}

			var recuringHolidays=document.CreateElement("RecuringHolidays");
			root.AppendChild(recuringHolidays);
			foreach(var holiday in AsSorted(this.RecuringHolidays))
			{
				var node=document.CreateElement("Date");
				node.InnerText=holiday.ToString();

				recuringHolidays.AppendChild(node);
			}

			return document;
		}

		private List<T> AsSorted<T>(ISet<T> values)
		{
			var list=new List<T>(values);
			list.Sort();

			return list;
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
