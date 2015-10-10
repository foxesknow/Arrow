using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("Invoke","Border")]
	public class InvokeBorder : ImageCmdlet
	{
		[Parameter(HelpMessage="The size of the border",Mandatory=true,ParameterSetName="Size")]
		public int Size{get;set;}

		[Parameter(HelpMessage="The width of the border",Mandatory=true,ParameterSetName="Both")]
		public int Width{get;set;}

		[Parameter(HelpMessage="The height of the border",Mandatory=true,ParameterSetName="Both")]
		public int Height{get;set;}

		protected override void Apply()
		{
			using(var image=GetMagickImage())
			{
				if(this.ParameterSetName=="Size")
				{
					image.Border(this.Size);
				}
				else
				{
					image.Border(this.Width,this.Height);
				}

				WriteImage(image);
			}
		}
	}
}
