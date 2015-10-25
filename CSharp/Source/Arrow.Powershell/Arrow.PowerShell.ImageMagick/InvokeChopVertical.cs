using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("Invoke","ChopVertical")]
	public class InvokeChopVertical : ImageCmdlet
	{
		[Parameter
		(
			Mandatory=true,
			HelpMessage="The Y offset from origin"
		)]
		public int Offset{get;set;}

		[Parameter
		(
			Mandatory=true,
			HelpMessage="The height of the part to chop vertically"
		)]
		public int Height{get;set;}

		protected override void Apply()
		{
			using(var image=GetMagickImage())
			{
				image.ChopVertical(this.Offset,this.Height);
				WriteImage(image);
			}
		}
	}
}
