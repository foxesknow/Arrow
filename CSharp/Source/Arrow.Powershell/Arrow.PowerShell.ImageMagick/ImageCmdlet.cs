using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace Arrow.PowerShell.ImageMagick
{
	public abstract class ImageCmdlet : PSCmdlet
	{
		[Parameter
		(
			Mandatory=true,
			ValueFromPipeline=true,
			ValueFromPipelineByPropertyName=true,
			HelpMessage="The image to manipulate"
		)]
		[System.Management.Automation.ValidateNotNull]
		public ImageData Image{get;set;}

		protected override sealed void ProcessRecord()
		{
			try
			{
				Apply();
			}
			catch(MagickCorruptImageErrorException e)
			{
				var error=new ErrorRecord(e,"Magick",ErrorCategory.InvalidData,this.Image);
				WriteError(error);
			}
			catch(MagickException e)
			{
				var error=new ErrorRecord(e,"Magick",ErrorCategory.NotSpecified,this.Image);
				WriteError(error);
			}
			catch(Exception e)
			{
				var error=new ErrorRecord(e,"Arrow.PowerShell.ImageMagick",ErrorCategory.NotSpecified,this.Image);
				WriteError(error);
			}
		}

		protected MagickImage GetMagickImage()
		{
			return new MagickImage(this.Image.Data);
		}

		protected void WriteImage(MagickImage magickImage)
		{
			var imageData=new ImageData()
			{
				File=this.Image.File,
				Data=magickImage.ToByteArray()
			};

			WriteObject(imageData);
		}

		protected abstract void Apply();
	}
}
