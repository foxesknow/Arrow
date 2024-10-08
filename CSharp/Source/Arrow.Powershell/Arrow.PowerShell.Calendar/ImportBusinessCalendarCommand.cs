﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using Arrow.Xml.ObjectCreation;

namespace Arrow.PowerShell.Calendar
{
	/// <summary>
	/// Imports an xml definition of a business calendar
	/// </summary>
	[Cmdlet("Import","BusinessCalendar")]
	[OutputType(typeof(BusinessCalendar))]
	public class ImportBusinessCalendarCommand : PSCmdlet
	{
		[Parameter
		(
			Mandatory=true,
			HelpMessage="The xml business calendar definition"
		)]
		[ValidateNotNullOrWhitespace]
		public string Filename{get;set;}

		protected override void ProcessRecord()
		{
			var filename=this.GetUnresolvedProviderPathFromPSPath(this.Filename);

			XmlDocument doc=new XmlDocument();
			doc.Load(filename);

			BusinessCalendar calendar=XmlCreation.Create<BusinessCalendar>(doc.DocumentElement);
			WriteObject(calendar);
		}
	}
}
