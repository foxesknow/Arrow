using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("Invoke","Chop")]
	public class InvokeChop : ImageCmdlet
	{
		[Parameter
		(
			Mandatory=true,
			ParameterSetName="Geometry",
			HelpMessage="The geometry to use"
		)]
		public MagickGeometry Geometry{get;set;}

		[Parameter
		(
			Mandatory=true,
			ParameterSetName="Rectangle",
			HelpMessage="The X offset from origin"
		)]
		public int XOffset{get;set;}

		[Parameter
		(
			Mandatory=true,
			ParameterSetName="Rectangle",
			HelpMessage="The Y offset from origin"
		)]
		public int YOffset{get;set;}

		[Parameter
		(
			Mandatory=true,
			ParameterSetName="Rectangle",
			HelpMessage="The width of the part to chop horizontally"
		)]
		public int Width{get;set;}

		[Parameter
		(
			Mandatory=true,
			ParameterSetName="Rectangle",
			HelpMessage="The height of the part to chop vertically"
		)]
		public int Height{get;set;}

		protected override void Apply()
		{
			using(var image=GetMagickImage())
			{
				if(this.ParameterSetName=="Geometry")
				{
					image.Chop(this.Geometry);
				}
				else
				{
					image.Chop(this.XOffset,this.Width,this.YOffset,this.Height);
				}
				
				WriteImage(image);
			}
		}
	}
}
