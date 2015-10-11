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

		[Parameter
		(
			Mandatory=false,
			HelpMessage="Additional options to layer onto the image"
		)]
		public ImageOptions ImageOptions{get;set;}

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
			var magick=new MagickImage(this.Image.Data);
			
			// First, apply the global options that are in the pipeline...
			if(this.Image.ImageOptions!=null) this.Image.ImageOptions.Apply(magick);

			// ...then apply any command specific ones
			if(this.ImageOptions!=null) this.ImageOptions.Apply(magick);

			return magick;
		}

		protected void WriteImage(MagickImage magickImage)
		{
			var imageData=new ImageData()
			{
				File=this.Image.File,
				Data=magickImage.ToByteArray(),
				ImageOptions=this.Image.ImageOptions
			};

			WriteObject(imageData);
		}

		protected abstract void Apply();
	}
}
