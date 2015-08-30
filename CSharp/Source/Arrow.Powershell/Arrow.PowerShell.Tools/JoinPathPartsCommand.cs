using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Arrow.PowerShell.Tools
{
    [Cmdlet(VerbsCommon.Join,"PathParts")]
	public class JoinPathPartsCommand : PSCmdlet
    {
		[Parameter
		(
			Mandatory=true,
			HelpMessage="The parts to combine to make a path"
		)]
		[ValidateNotNull]
		public string[] Parts{get;set;}

		protected override void ProcessRecord()
		{
			var path=Path.Combine(this.Parts);
			WriteObject(path);
		}
    }
}
