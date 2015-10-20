using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("Invoke","ChangeColorSpace")]
	public class InvokeCharcoal : ImageCmdlet
	{
		public InvokeCharcoal()
		{
			this.Radius=0;
			this.Sigma=1;
		}

		[Parameter
		(
			HelpMessage="The radius of the Gaussian, in pixels, not counting the center pixel"
		)]
		public double Radius{get;set;}

		[Parameter
		(
			HelpMessage="The standard deviation of the Laplacian, in pixels"
		)]
		public double Sigma{get;set;}

		protected override void Apply()
		{
			using(var image=GetMagickImage())
			{
				image.Charcoal(this.Radius,this.Sigma);
				WriteImage(image);
			}
		}
	}
}
