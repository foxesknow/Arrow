using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("Invoke","CopyPixels")]
	public class InvokeCopyPixels : ImageCmdlet
	{
		[Parameter(Mandatory=true,HelpMessage="The source image")]
		public ImageData Source{get;set;}

		[Parameter(Mandatory=true,HelpMessage="The geometry")]
		public MagickGeometry Geometry{get;set;}

		[Parameter(Mandatory=true,HelpMessage="The offset")]
		public Coordinate Offset{get;set;}

		protected override void Apply()
		{
			using(var image=GetMagickImage())
			{
				var source=new MagickImage(this.Source.Data);
				image.CopyPixels(source,this.Geometry,this.Offset);
				WriteImage(image);
			}
		}
	}
}

