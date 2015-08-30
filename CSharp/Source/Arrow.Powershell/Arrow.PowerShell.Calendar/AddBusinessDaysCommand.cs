using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.PowerShell.Calendar
{
	/// <summary>
	/// Adds a number of days to a date, using a business calendar as reference
	/// </summary>
	[Cmdlet(VerbsCommon.Add,"BusinessDays")]
	[OutputType(typeof(DateTime))]
	public class AddBusinessDaysCommand : BusinessCalendarCmdlet
	{	
		[Parameter
		(
			Mandatory=true,
			HelpMessage="The number of days to add"
		)]
		public int Add{get;set;}

		protected override void ProcessRecord()
		{
			int direction=(this.Add<0 ? -1 : 1);
			int days=Math.Abs(this.Add);

			var calendar=this.BusinessCalendar;
			var date=this.Date;

			while(days!=0)
			{
				date=date.AddDays(direction);

				while(calendar.IsBusinessDate(date)==false)
				{
					date=date.AddDays(direction);
				}

				days--;
			}

			WriteObject(date);
		}
	}
}
