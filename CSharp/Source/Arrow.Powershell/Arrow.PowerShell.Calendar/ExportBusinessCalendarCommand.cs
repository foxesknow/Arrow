using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Arrow.PowerShell.Calendar
{
	/// <summary>
	/// Exports a business calendar object to an xml document
	/// </summary>
	[Cmdlet("Export","BusinessCalendar")]
	[OutputType(typeof(XmlDocument))]
	public class ExportBusinessCalendarCommand : PSCmdlet
	{
		[Parameter
		(
			HelpMessage="The business calendar to export",
			ValueFromPipeline=true,
			Mandatory=true
		)]
		[ValidateNotNull]
		public BusinessCalendar BusinessCalendar{get;set;}

		protected override void ProcessRecord()
		{
			var document=this.BusinessCalendar.ToXml();
			WriteObject(document);
		}
	}
}
