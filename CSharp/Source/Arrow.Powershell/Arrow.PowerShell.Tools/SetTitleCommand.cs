using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.PowerShell.Tools
{
	[Cmdlet(VerbsCommon.Set,"Title")]
	[OutputType(typeof(string))]
	public class SetTitleCommand : PSCmdlet
	{
		[Parameter
		(
			Mandatory=true,
			HelpMessage="The title for the PowerShell window",
			ValueFromPipeline=true,
			ValueFromPipelineByPropertyName=true,
			Position=0
		)]
		[ValidateNotNullOrWhitespace]
		public string Title{get;set;}

		protected override void ProcessRecord()
		{
			this.Host.UI.RawUI.WindowTitle=this.Title;
			WriteObject(this.Title);
		}
	}
}
