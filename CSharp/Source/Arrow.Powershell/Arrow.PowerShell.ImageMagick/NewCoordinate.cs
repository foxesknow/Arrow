using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("New","Coordinate")]
	[OutputType(typeof(Coordinate))]
	public class NewCoordinate : PSCmdlet
	{
		[Parameter(Mandatory=true,HelpMessage="The X position of the coordinate")]
		public double X{get;set;}

		[Parameter(Mandatory=true,HelpMessage="The Y position of the coordinate")]
		public double Y{get;set;}

		protected override void ProcessRecord()
		{
			var c=new Coordinate(this.X,this.Y);
			WriteObject(c);
		}
	}
}
