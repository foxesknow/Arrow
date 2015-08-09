using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("Invoke","AdaptiveSharpen")]
	public class InvokeAdaptiveSharpen : ImageCmdlet
	{
		public InvokeAdaptiveSharpen()
		{
			this.Radius=0;
			this.Sigma=1;
		}

		[Parameter(HelpMessage="The radius of the Gaussian, in pixels, not counting the center pixel")]
		public double Radius{get;set;}

		[Parameter(HelpMessage="The standard deviation of the Laplacian, in pixels")]
		public double Sigma{get;set;}

		[Parameter(HelpMessage="The channels to sharpen")]
		public Channels Channels{get;set;}

		protected override void Apply()
		{
			using(var image=GetMagickImage())
			{
				if(this.Channels==global::ImageMagick.Channels.Undefined)
				{
					image.AdaptiveSharpen(this.Radius,this.Sigma);
				}
				else
				{
					image.AdaptiveSharpen(this.Radius,this.Sigma,this.Channels);
				}

				WriteImage(image);
			}
		}
	}
}
