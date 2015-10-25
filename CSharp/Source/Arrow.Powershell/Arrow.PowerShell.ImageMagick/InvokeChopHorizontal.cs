using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("Invoke","ChopHorizontal")]
	public class InvokeChopHorizontal : ImageCmdlet
	{
		[Parameter
		(
			Mandatory=true,
			HelpMessage="The X offset from origin"
		)]
		public int Offset{get;set;}

		[Parameter
		(
			Mandatory=true,
			HelpMessage="The width of the part to chop horizontally"
		)]
		public int Width{get;set;}

		protected override void Apply()
		{
			using(var image=GetMagickImage())
			{
				image.ChopHorizontal(this.Offset,this.Width);
				WriteImage(image);
			}
		}
	}
}
