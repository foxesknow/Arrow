using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("Invoke","Convolve")]
	public class InvokeConvolve : ImageCmdlet
	{
		[Parameter(Mandatory=true,HelpMessage="The matrix to apply")]
		public ConvolveMatrix ConvolveMatrix{get;set;}

		protected override void Apply()
		{
			using(var image=GetMagickImage())
			{
				image.Convolve(this.ConvolveMatrix);
				WriteImage(image);
			}
		}
	}
}
