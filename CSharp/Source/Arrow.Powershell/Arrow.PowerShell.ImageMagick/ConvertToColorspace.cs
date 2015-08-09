using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("ConvertTo","Colorspace")]
	public class ConvertToColorspace : ImageCmdlet
	{
		[Parameter
		(
			Mandatory=true,
			Position=0,
			HelpMessage="The colorspace to convert to"
		)]
		public ColorSpace Colorspace{get;set;}

		protected override void Apply()
		{
			using(var image=GetMagickImage())
			{
				image.ColorSpace=this.Colorspace;

				WriteImage(image);	
			}
		}
	}
}
