using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.PowerShell.Calendar
{
	/// <summary>
	/// Base class for commands that a business calendar
	/// </summary>
	public abstract class BusinessCalendarCmdlet : DateCmdlet
	{
		protected BusinessCalendarCmdlet()
		{
			// Create a default calendar with the weekends filled in
			this.BusinessCalendar=new BusinessCalendar();
			this.BusinessCalendar.Weekends.Add(DayOfWeek.Saturday);
			this.BusinessCalendar.Weekends.Add(DayOfWeek.Sunday);
		}

		[Parameter(HelpMessage="The business calendar to use")]
		[ValidateNotNull]
		public BusinessCalendar BusinessCalendar{get;set;}
	}
}
