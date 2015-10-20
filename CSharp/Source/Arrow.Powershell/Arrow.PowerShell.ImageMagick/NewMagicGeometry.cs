using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("New","MagicGeometry")]
	[OutputType(typeof(MagickGeometry))]
	public class NewMagicGeometry : PSCmdlet
	{
		[Parameter(ParameterSetName="WidthAndHeight",Mandatory=true,HelpMessage="The width and height")]
		public int WidthAndHeight{get;set;}

		[Parameter(ParameterSetName="WidthHeight",Mandatory=true,HelpMessage="The width")]
		[Parameter(ParameterSetName="XYWidthHeight",Mandatory=true,HelpMessage="The width")]
		public int Width{get;set;}

		[Parameter(ParameterSetName="WidthHeight",Mandatory=true,HelpMessage="The height")]
		[Parameter(ParameterSetName="XYWidthHeight",Mandatory=true,HelpMessage="The height")]
		public int Height{get;set;}

		[Parameter(ParameterSetName="PercentageWidthHeight",Mandatory=true,HelpMessage="The percentage of the width")]
		[Parameter(ParameterSetName="XYPercentageWidthHeight",Mandatory=true,HelpMessage="The percentage of the width")]
		public double PercentageWidth{get;set;}

		[Parameter(ParameterSetName="PercentageWidthHeight",Mandatory=true,HelpMessage="The percentage of the height")]
		[Parameter(ParameterSetName="XYPercentageWidthHeight",Mandatory=true,HelpMessage="The percentage of the height")]
		public double PercentageHeight{get;set;}

		[Parameter(ParameterSetName="XYWidthHeight",Mandatory=true,HelpMessage="The X offset from origin")]
		[Parameter(ParameterSetName="XYPercentageWidthHeight",Mandatory=true,HelpMessage="The X offset from origin")]
		public int X{get;set;}

		[Parameter(ParameterSetName="XYWidthHeight",Mandatory=true,HelpMessage="The Y offset from origin")]
		[Parameter(ParameterSetName="XYPercentageWidthHeight",Mandatory=true,HelpMessage="The Y offset from origin")]
		public int Y{get;set;}

		protected override void ProcessRecord()
		{
			switch(this.ParameterSetName)
			{
				case "WidthAndHeight":
					WriteObject(new MagickGeometry(this.WidthAndHeight));
					break;

				case "WidthHeight":
					WriteObject(new MagickGeometry(this.Width,this.Height));
					break;

				case "PercentageWidthHeight":
					WriteObject(new MagickGeometry(new Percentage(this.PercentageWidth),new Percentage(this.PercentageHeight)));
					break;

				case "XYWidthHeight":
					WriteObject(new MagickGeometry(this.X,this.Y,this.Width,this.Height));
					break;

				case "XYPercentageWidthHeight":
					WriteObject(new MagickGeometry(this.X,this.Y,new Percentage(this.PercentageWidth),new Percentage(this.PercentageHeight)));
					break;

				default:
					throw new InvalidOperationException("Unknown parameter set: "+this.ParameterSetName);
					break;
			}
		}
	}
}
