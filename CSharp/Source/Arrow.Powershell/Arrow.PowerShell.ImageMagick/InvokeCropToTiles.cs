using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("Invoke","CropToTiles")]
	public class InvokeCropToTiles : ImageCmdlet
	{
		[Parameter(Mandatory=true,ParameterSetName="Geometry",HelpMessage="The size of the tile")]
		public MagickGeometry Geometry{get;set;}

		[Parameter(Mandatory=true,ParameterSetName="WidthHeight",HelpMessage="The width of the tile")]
		public int Width{get;set;}

		[Parameter(Mandatory=true,ParameterSetName="WidthHeight",HelpMessage="The height of the tile")]
		public int Height{get;set;}

		protected override void Apply()
		{
			IEnumerable<MagickImage> tiles=null;

			using(var image=GetMagickImage())
			{
				switch(this.ParameterSetName)
				{
					case "Geometry":
						tiles=image.CropToTiles(this.Geometry);
						break;

					case "WidthHeight":
						tiles=image.CropToTiles(this.Width,this.Height);
						break;

					default:
						throw new InvalidOperationException("Unknown parameter set: "+this.ParameterSetName);
				}

				foreach(var tile in tiles)
				{
					WriteImage(tile);
				}
			}
		}
	}
}
