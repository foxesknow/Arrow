using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("Invoke","ContrastStretch")]
	public class InvokeContrastStretch : ImageCmdlet
	{
		[Parameter(Mandatory=true,ParameterSetName="BPoint",HelpMessage="The black point")]
		[Parameter(Mandatory=true,ParameterSetName="BWPoint",HelpMessage="The black point")]
		[Parameter(Mandatory=true,ParameterSetName="BWCPoint",HelpMessage="The black point")]
		public double BlackPoint{get;set;}

		[Parameter(Mandatory=true,ParameterSetName="BWPoint",HelpMessage="The black point")]
		[Parameter(Mandatory=true,ParameterSetName="BWCPoint",HelpMessage="The white point")]
		public double WhitekPoint{get;set;}

		[Parameter(Mandatory=true,ParameterSetName="BWCPoint",HelpMessage="The channels to contrast stretch")]
		public Channels Channels{get;set;}

		protected override void Apply()
		{
			using(var image=GetMagickImage())
			{
				switch(this.ParameterSetName)
				{
					case "BPoint":
						image.ContrastStretch((Percentage)this.BlackPoint);
						break;

					case "BWPoint":
						image.ContrastStretch((Percentage)this.BlackPoint,(Percentage)this.WhitekPoint);
						break;

					case "BWCPoint":
						image.ContrastStretch((Percentage)this.BlackPoint,(Percentage)this.WhitekPoint,this.Channels);
						break;

					default:
						throw new InvalidOperationException("Unknown parameter set: "+this.ParameterSetName);
				}

				WriteImage(image);
			}
		}
	}
}
