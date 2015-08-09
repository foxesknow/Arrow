using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("Invoke","AddNoise")]
	public class InvokeAddNoise : ImageCmdlet
	{
		[Parameter
		(
			Mandatory=true,
			HelpMessage="The type of noise to add"
		)]
		public NoiseType NoiseType{get;set;}

		[Parameter(HelpMessage="The channgels to add the noise to")]
		public Channels Channels{get;set;}

		protected override void Apply()
		{
			using(var image=GetMagickImage())
			{
				if(this.Channels==global::ImageMagick.Channels.Undefined)
				{
					image.AddNoise(this.NoiseType);
				}
				else
				{
					image.AddNoise(this.NoiseType,this.Channels);
				}

				WriteImage(image);
			}
		}
	}
}
