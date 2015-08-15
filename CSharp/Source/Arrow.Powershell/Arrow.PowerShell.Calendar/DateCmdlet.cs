using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.PowerShell.Calendar
{
	public abstract class DateCmdlet : PSCmdlet
	{
		protected DateCmdlet()
		{
			this.Date=DateTime.Now;
		}

		[Parameter
		(
			ValueFromPipeline=true,
			HelpMessage="The reference date to use"
		)]
		public DateTime Date{get;set;}
	}
}
