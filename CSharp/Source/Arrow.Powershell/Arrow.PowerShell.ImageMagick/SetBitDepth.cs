using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("Set","BitDepth")]
	public class SetBitDepth : ImageCmdlet
	{
		[Parameter
		(
			Mandatory=true,
			HelpMessage="The channels to set the depth for"
		)]
		public int Depth{get;set;}
		
		[Parameter(HelpMessage="The channel to set the depth for")]
		public Channels Channels{get;set;}

		protected override void Apply()
		{
			using(var image=GetMagickImage())
			{
				if(this.Channels==global::ImageMagick.Channels.Undefined)
				{
					image.BitDepth(this.Depth);
				}
				else
				{
					image.BitDepth(this.Channels,this.Depth);
				}

				WriteImage(image);
			}
		}
	}
}
