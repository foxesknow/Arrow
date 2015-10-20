using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("Invoke","CannyEdge")]
	public class InvokeCannyEdge : ImageCmdlet
	{
		public InvokeCannyEdge()
		{
			this.Radius=0;
			this.Sigma=1;
			this.Lower=10;
			this.Upper=30;
		}

		[Parameter
		(
			Mandatory=false,
			HelpMessage="The radius of the gaussian smoothing filter"
		)]
		public double Radius{get;set;}

		[Parameter
		(
			Mandatory=false,
			HelpMessage="The sigma of the gaussian smoothing filter"
		)]
		public double Sigma{get;set;}

		[Parameter
		(
			Mandatory=false,
			HelpMessage="Percentage of edge pixels in the lower threshold"
		)]
		public double Lower{get;set;}

		[Parameter
		(
			Mandatory=false,
			HelpMessage="Percentage of edge pixels in the upper threshold"
		)]
		public double Upper{get;set;}

		protected override void Apply()
		{
			using(var image=GetMagickImage())
			{
				image.CannyEdge(this.Radius,this.Sigma,new Percentage(this.Lower),new Percentage(this.Upper));

				WriteImage(image);
			}
		}
	}
}
