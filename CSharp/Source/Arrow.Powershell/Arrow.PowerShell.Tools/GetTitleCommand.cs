using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.PowerShell.Tools
{
	[Cmdlet(VerbsCommon.Get,"Title")]
	[OutputType(typeof(string))]
	public class GetTitleCommand : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			WriteObject(this.Host.UI.RawUI.WindowTitle);
		}
	}
}
