using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("Invoke","ChangeColorSpace")]
	public class InvokeChangeColorSpace : ImageCmdlet
	{
		[Parameter
		(
			Mandatory=true,
			HelpMessage="The colorspace to apply"
		)]
		public ColorSpace ColorSpace{get;set;}

		protected override void Apply()
		{
			using(var image=GetMagickImage())
			{
				image.ChangeColorSpace(this.ColorSpace);
				WriteImage(image);
			}
		}
	}
}
