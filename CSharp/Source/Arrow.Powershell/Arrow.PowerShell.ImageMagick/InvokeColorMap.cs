using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("Invoke","ColorMap")]
	public class InvokeColorMap : ImageCmdlet
	{
		[Parameter(Mandatory=true,HelpMessage="The position index")]
		public int Index{get;set;}

		[Parameter(Mandatory=true,HelpMessage="The color")]
		public MagickColor Color{get;set;}

		protected override void Apply()
		{
			using(var image=GetMagickImage())
			{
				image.ColorMap(this.Index,this.Color);
				WriteImage(image);
			}
		}
	}
}
