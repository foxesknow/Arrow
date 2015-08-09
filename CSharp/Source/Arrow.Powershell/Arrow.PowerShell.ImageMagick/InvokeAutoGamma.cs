using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("Invoke","AutoGamma")]
	public class InvokeAutoGamma : ImageCmdlet
	{
		[Parameter(HelpMessage="The channels to set the gamma for")]
		public Channels Channels{get;set;}

		protected override void Apply()
		{
			using(var image=GetMagickImage())
			{
				if(this.Channels==global::ImageMagick.Channels.Undefined)
				{
					image.AutoGamma();
				}
				else
				{
					image.AutoGamma(this.Channels);
				}

				WriteImage(image);
			}
		}
	}
}
