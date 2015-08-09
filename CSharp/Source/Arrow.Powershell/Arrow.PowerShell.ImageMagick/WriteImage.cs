using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("Write","Image")]
	public class WriteImage : ImageCmdlet
	{
		protected override void Apply()
		{
			using(var image=GetMagickImage())
			{
				image.Write(this.Image.File);
			}
		}
	}
}
 