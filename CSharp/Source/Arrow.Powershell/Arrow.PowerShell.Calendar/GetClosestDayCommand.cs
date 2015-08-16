using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.PowerShell.Calendar
{
	/// <summary>
	/// Returns the closest date to the supplied date
	/// For example, if today is 16th August 2015 (a Sunday) and you
	/// set DayOfWeek to Wednesday it will return 19th August 2015
	/// </summary>
	[Cmdlet(VerbsCommon.Get,"ClosestDay")]
	[OutputType(typeof(DateTime))]
	public class GetClosestDayCommand : DateCmdlet
	{
		private bool m_Backwards;
		private bool m_IncludeDate;

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
		public SwitchParameter Backwards
		{
			get{return m_Backwards;}
			set{m_Backwards=value;}
		}

		[Parameter
		(
			HelpMessage="Includes the supplied date as part of the calculation"
		)]
		public SwitchParameter IncludeDate
		{
			get{return m_IncludeDate;}
			set{m_IncludeDate=value;}
		}

		protected override void ProcessRecord()
		{
			var date=this.Date;
			int delta=(m_Backwards ? -1 : 1);

			if(m_IncludeDate==false)
			{
				date=date.AddDays(delta);
			}

			while(date.DayOfWeek!=this.DayOfWeek)
			{
				date=date.AddDays(delta);
			}

			WriteObject(date);
		}
	}
}
