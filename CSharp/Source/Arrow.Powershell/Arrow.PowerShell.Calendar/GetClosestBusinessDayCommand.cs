using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.PowerShell.Calendar
{
	/// <summary>
	/// Returns the closest business day to a specified date
	/// </summary>
	[Cmdlet(VerbsCommon.Get,"ClosestBusinessDay")]
	[OutputType(typeof(DateTime))]
	public class GetClosestBusinessDayCommand : BusinessCalendarCmdlet
	{
		[Parameter
		(
			HelpMessage="The day of the week to get",
			Mandatory=true
		)]
		public DayOfWeek DayOfWeek{get;set;}

		[Parameter
		(
			HelpMessage="Goes backwards rather than forwards"
		)]
		public SwitchParameter Backwards{get;set;}

		[Parameter
		(
			HelpMessage="Includes the supplied date as part of the calculation"
		)]
		public SwitchParameter IncludeDate{get;set;}

		protected override void ProcessRecord()
		{
			if(this.BusinessCalendar.Weekends.Contains(this.DayOfWeek))
			{
				// We'll never find a matching day as we're always skipping weekends, so fail
				throw new ArgumentException("Cannot find the closest weekend!");
			}

			var date=this.Date;
			int delta=(this.Backwards ? -1 : 1);

			if(this.IncludeDate==false)
			{
				date=date.AddDays(delta);
			}

			while(date.DayOfWeek!=this.DayOfWeek || this.BusinessCalendar.IsBusinessDate(date)==false)
			{
				date=date.AddDays(delta);
			}

			WriteObject(date);
		}
	}
}
