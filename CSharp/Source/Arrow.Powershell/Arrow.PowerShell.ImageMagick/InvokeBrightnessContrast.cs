using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("Invoke","BrightnessContrast")]
	public class InvokeBrightnessContrast : ImageCmdlet
	{
		[Parameter
		(
			Mandatory=true,
			HelpMessage="The percentage to adjust the brightness by"
		)]
		public double Brightness{get;set;}

		[Parameter
		(
			Mandatory=true,
			HelpMessage="The percentage to adjust the brightness by"
		)]
		public double Contrast{get;set;}

		[Parameter
		(
			Mandatory=true,
			HelpMessage="The channels to apply the settings to"
		)]
		public Channels? Channels{get;set;}

		protected override void Apply()
		{
			using(var image=GetMagickImage())
			{
				var b=new Percentage(this.Brightness);
				var c=new Percentage(this.Contrast);

				if(this.Channels.HasValue)
				{
					image.BrightnessContrast(b,c,this.Channels.Value);
				}
				else
				{
					image.BrightnessContrast(b,c);
				}

				WriteImage(image);
			}
		}
	}
}
