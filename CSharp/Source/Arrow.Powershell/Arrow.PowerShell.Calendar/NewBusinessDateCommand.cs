using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.PowerShell.Calendar
{
	[Cmdlet(VerbsCommon.New,"BusinessDate")]
	[OutputType(typeof(BusinessDate))]
	public class NewBusinessDateCommand : PSCmdlet
	{
		[Parameter
		(
			Mandatory=true,
			ParameterSetName="Parts",
			HelpMessage="The year"
		)]
		public int Year{get;set;}

		[Parameter
		(
			Mandatory=true,
			ParameterSetName="Parts",
			HelpMessage="The month"
		)]
		public int Month{get;set;}

		[Parameter
		(
			Mandatory=true,
			ParameterSetName="Parts",
			HelpMessage="The day"
		)]
		public int Day{get;set;}

		[Parameter
		(
			Mandatory=true,
			ParameterSetName="DateTime",
			ValueFromPipeline=true,
			HelpMessage="The date"
		)]
		public DateTime Date{get;set;}

		protected override void ProcessRecord()
		{
			if(this.ParameterSetName=="Parts")
			{
				var date=new DateTime(this.Year,this.Month,this.Day);
				var businessDate=new BusinessDate(date.Year,date.Month,date.Day);
				WriteObject(businessDate);
			}
			else
			{
				var date=this.Date;
				var businessDate=new BusinessDate(date.Year,date.Month,date.Day);
				WriteObject(businessDate);
			}
		}
	}
}
