using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("Set","BlackThreshold")]
	public class SetBlackThreshold : ImageCmdlet
	{
		[Parameter(HelpMessage="The threshold to use")]
		public double Threshold{get;set;}

		[Parameter(HelpMessage="The channel to set make black")]
		public Channels Channels{get;set;}

		protected override void Apply()
		{
			using(var image=GetMagickImage())
			{
				if(this.Channels==global::ImageMagick.Channels.Undefined)
				{
					image.BlackThreshold(this.Threshold);
				}
				else
				{
					image.BlackThreshold(this.Threshold,this.Channels);
				}

				WriteImage(image);
			}
		}
	}
}
