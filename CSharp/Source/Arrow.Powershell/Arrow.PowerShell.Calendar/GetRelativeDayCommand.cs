using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.PowerShell.Calendar
{
	/// <summary>
	/// Returns a day in the supplied month that is specified in a relative way
	/// </summary>
	[Cmdlet(VerbsCommon.Get,"RelativeDay")]
	[OutputType(typeof(DateTime))]
	public class GetRelativeDayCommand : DateCmdlet
	{
		private bool m_Backwards;

		[Parameter
		(
			ParameterSetName="RelativeDay",
			HelpMessage="The relative day to get",
			Mandatory=true
		)]
		public RelativeDayOfWeek RelativeDay{get;set;}

		[Parameter
		(
			ParameterSetName="DayOfWeek",
			HelpMessage="The day of the week to get",
			Mandatory=true
		)]
		public DayOfWeek DayOfWeek{get;set;}

		[Parameter
		(
			ParameterSetName="DayOfWeek",
			HelpMessage="The week of the month to get",
			Mandatory=true
		)]
		public Week Week{get;set;}

		[Parameter
		(
			HelpMessage="Goes backwards rather than forwards"
		)]
		public SwitchParameter Backwards
		{
			get{return m_Backwards;}
			set{m_Backwards=value;}
		}

		protected override void ProcessRecord()
		{
			DayOfWeek dayOfWeek;
			int weekOffset;

			ExtractCommandData(out dayOfWeek, out weekOffset);

			DateTime baseline;
			
			if(m_Backwards)
			{
				// We're going from the end, so the week offset it negative
				weekOffset*=-1;
				baseline=this.Date.GetLastDayOfWeek(dayOfWeek);
			}
			else
			{
				baseline=this.Date.GetFirstDayOfWeek(dayOfWeek);
			}

			var weekAdjusted=baseline.AddDays(7*weekOffset);

			// Make sure we're still in the same month
			if(weekAdjusted.Year==this.Date.Year && weekAdjusted.Month==this.Date.Month)
			{
				WriteObject(weekAdjusted);
			}
			else
			{
				var error=new ErrorRecord
				(
					new ArgumentException("invalid relative day for month"),
					"Date",
					ErrorCategory.InvalidArgument,
					this.RelativeDay
				);

				WriteError(error);
			}			
		}

		private void ExtractCommandData(out DayOfWeek dayOfWeek, out int weekOffset)
		{
			if(this.ParameterSetName=="RelativeDay")
			{
				dayOfWeek=this.RelativeDay.GetDayOfWeek();
				weekOffset=this.RelativeDay.GetWeekOffset();
			}
			else
			{
				dayOfWeek=this.DayOfWeek;
				weekOffset=(int)this.Week;
			}
		}
	}
}
