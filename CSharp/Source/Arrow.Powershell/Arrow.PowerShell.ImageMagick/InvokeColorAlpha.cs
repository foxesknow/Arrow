using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("Invoke","ColorAlpha")]
	public class InvokeColorAlpha : ImageCmdlet
	{
		[Parameter
		(
			Mandatory=true,
			HelpMessage="The color to the alpha channel to"
		)]
		public MagickColor Color{get;set;}

		protected override void Apply()
		{
			using(var image=GetMagickImage())
			{
				image.ColorAlpha(this.Color);
				WriteImage(image);
			}
		}
	}
}
