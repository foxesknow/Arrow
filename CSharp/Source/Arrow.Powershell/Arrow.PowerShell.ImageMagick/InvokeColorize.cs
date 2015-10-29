using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("Invoke","Colorize")]
	public class InvokeColorize : ImageCmdlet
	{
		[Parameter(Mandatory=true,ParameterSetName="Alpha",HelpMessage="The color to use")]
		[Parameter(Mandatory=true,ParameterSetName="AlphaRGB",HelpMessage="The color to use")]
		public MagickColor Color{get;set;}

		[Parameter(Mandatory=true,ParameterSetName="Alpha",HelpMessage="The alpha percentage")]
		public double Alpha{get;set;}

		[Parameter(Mandatory=true,ParameterSetName="AlphaRGB",HelpMessage="The alpha percentage")]
		public double AlphaRed{get;set;}

		[Parameter(Mandatory=true,ParameterSetName="AlphaRGB",HelpMessage="The alpha percentage")]
		public double AlphaGreen{get;set;}

		[Parameter(Mandatory=true,ParameterSetName="AlphaRGB",HelpMessage="The alpha percentage")]
		public double AlphaBlue{get;set;}

		protected override void Apply()
		{
			using(var image=GetMagickImage())
			{
				switch(this.ParameterSetName)
				{
					case "Alpha":
						image.Colorize(this.Color,new Percentage(this.Alpha));
						break;

					case "AlphaRGB":
						image.Colorize(this.Color,new Percentage(this.AlphaRed),new Percentage(this.AlphaGreen),new Percentage(this.AlphaBlue));
						break;

					default:
						throw new InvalidOperationException("Unknown parameter set: "+this.ParameterSetName);
				}
				
				WriteImage(image);
			}
		}
	}
}
