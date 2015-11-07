using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("Invoke","Crop")]
	public class InvokeCrop : ImageCmdlet
	{
		[Parameter(Mandatory=true,ParameterSetName="Geometry",HelpMessage="The geometry")]
		public MagickGeometry Geometry{get;set;}

		[Parameter(Mandatory=true,ParameterSetName="WidthHeight",HelpMessage="The width of the subregion")]
		[Parameter(Mandatory=true,ParameterSetName="WidthHeightGravity",HelpMessage="The width of the subregion")]
		[Parameter(Mandatory=true,ParameterSetName="XYWidthHeight",HelpMessage="The width of the subregion")]
		public int Width{get;set;}

		[Parameter(Mandatory=true,ParameterSetName="WidthHeight",HelpMessage="The height of the subregion")]
		[Parameter(Mandatory=true,ParameterSetName="WidthHeightGravity",HelpMessage="The height of the subregion")]
		[Parameter(Mandatory=true,ParameterSetName="XYWidthHeight",HelpMessage="The height of the subregion")]
		public int Height{get;set;}

		[Parameter(Mandatory=true,ParameterSetName="WidthHeightGravity",HelpMessage="The position where the cropping should start from")]
		public Gravity Gravity{get;set;}

		[Parameter(Mandatory=true,ParameterSetName="XYWidthHeight",HelpMessage="The X offset from origin")]
		public int X{get;set;}

		[Parameter(Mandatory=true,ParameterSetName="XYWidthHeight",HelpMessage="The Y offset from origin")]
		public int Y{get;set;}

		protected override void Apply()
		{
			using(var image=GetMagickImage())
			{
				switch(this.ParameterSetName)
				{
					case "Geometry":
						image.Crop(this.Geometry);
						break;

					case "WidthHeight":
						image.Crop(this.Width,this.Height);
						break;

					case "WidthHeightGravity":
						image.Crop(this.Width,this.Height,this.Gravity);
						break;

					case "XYWidthHeight":
						image.Crop(this.X,this.Y,this.Width,this.Height);
						break;

					default:
						throw new InvalidOperationException("Unknown parameter set: "+this.ParameterSetName);
				}

				WriteImage(image);
			}
		}
	}
}
