﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.PowerShell.Calendar
{
	/// <summary>
	/// Adds a new date to a calendar
	/// </summary>
	[Cmdlet(VerbsCommon.Add,"BusinessDate")]
	[OutputType(typeof(BusinessCalendar))]
	public class AddBusinessDateCommand : PSCmdlet
	{
		[Parameter
		(
			HelpMessage="The business calendar to use",
			Mandatory=true
		)]
		[ValidateNotNull]
		public BusinessCalendar BusinessCalendar{get;set;}

		[Parameter
		(
			Mandatory=true,
			ValueFromPipeline=true,
			HelpMessage="The date to add to the calendar"
		)]
		public BusinessDate Date{get;set;}

		[Parameter
		(
			Mandatory=true,
			HelpMessage="Indicates a one-off holiday",
			ParameterSetName="OneOff"
		)]
		public SwitchParameter Holiday{get;set;}

		[Parameter
		(
			Mandatory=true,
			HelpMessage="Indicates a recuring holiday",
			ParameterSetName="Recuring"
		)]
		public SwitchParameter RecuringHoliday{get;set;}

		protected override void ProcessRecord()
		{
			if(this.ParameterSetName=="OneOff")
			{
				this.BusinessCalendar.Holidays.Add(this.Date);
			}
			else
			{
				this.BusinessCalendar.RecuringHolidays.Add(this.Date);
			}

			WriteObject(this.BusinessCalendar);
		}
	}
}
