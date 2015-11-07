using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("Invoke","Contrast")]
	public class InvokeContrast : ImageCmdlet
	{
		[Parameter
		(
			Mandatory=true,
			HelpMessage="True to enhance the contrast and false to reduce the contrast"
		)]
		public bool? Enhance{get;set;}

		protected override void Apply()
		{
			using(var image=GetMagickImage())
			{
				if(this.Enhance.HasValue)
				{
					image.Contrast(this.Enhance.Value);
				}
				else
				{
					image.Contrast();
				}

				WriteImage(image);
			}
		}
	}
}
