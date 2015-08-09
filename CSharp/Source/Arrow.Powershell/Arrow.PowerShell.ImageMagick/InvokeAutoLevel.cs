using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("Invoke","AutoLevel")]
	public class InvokeAutoLevel : ImageCmdlet
	{
		[Parameter(HelpMessage="The channels to level")]
		public Channels Channels{get;set;}

		protected override void Apply()
		{
			using(var image=GetMagickImage())
			{
				if(this.Channels==global::ImageMagick.Channels.Undefined)
				{
					image.AutoLevel();
				}
				else
				{
					image.AutoLevel(this.Channels);
				}

				WriteImage(image);
			}
		}
	}
}
