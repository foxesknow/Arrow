using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("Invoke","Resize")]
	public class InvokeResize : ImageCmdlet
	{
		// Read-Image .\DSC_9139.jpg | ConvertTo-Format bmp | Invoke-Resize -Width 500 | Write-Image

		private bool m_IgnoreAspectRatio;

		[Parameter(HelpMessage="Indicates if the aspect ration should be ignored when resizing")]
		public SwitchParameter IgnoreAspectRatio
		{
			get{return m_IgnoreAspectRatio;}
			set{m_IgnoreAspectRatio=value;}
		}

		[Parameter
		(
			ParameterSetName="WidthHeight",
			HelpMessage="The width of the image"
		)]
		public int? Width{get;set;}

		[Parameter
		(
			ParameterSetName="WidthHeight",
			HelpMessage="The height of the image"
		)]
		public int? Height{get;set;}

		[Parameter
		(
			ParameterSetName="Percentage",
			HelpMessage="The percentage to resize the image by"
		)]
		public double? Percentage{get;set;}

		[Parameter
		(
			ParameterSetName="PercentageWidthHeight",
			HelpMessage="The percentage change to the width of the image"
		)]
		public double? PercentageWidth{get;set;}

		[Parameter
		(
			ParameterSetName="PercentageWidthHeight",
			HelpMessage="The percentage change to the height of the image"
		)]
		public double? PercentageHeight{get;set;}

		protected override void Apply()
		{
			using(var image=GetMagickImage())
			{
				MagickGeometry geometry=null;

				if(this.Percentage!=null)
				{
					geometry=new MagickGeometry(this.Percentage.Value,this.Percentage.Value);
				}
				else if(this.PercentageWidth!=null || this.PercentageHeight!=null)
				{
					geometry=new MagickGeometry(new Percentage(this.PercentageWidth.GetValueOrDefault()),new Percentage(this.PercentageHeight.GetValueOrDefault()));
					
				}
				else if(this.Width!=null || this.Height!=null)
				{
					geometry=new MagickGeometry(this.Width.GetValueOrDefault(),this.Height.GetValueOrDefault());
				}

				if(geometry!=null)
				{
					geometry.IgnoreAspectRatio=m_IgnoreAspectRatio;
					image.Resize(geometry);
				}

				WriteImage(image);				
			}
		}
	}
}
