using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace Arrow.PowerShell.ImageMagick
{
    [Cmdlet("Read","Image",DefaultParameterSetName="Filename")]
	public class ReadImage : PSCmdlet
    {
		[Parameter
		(
			Mandatory=true,
			ValueFromPipeline=true,
			ParameterSetName="FileInfo",
			HelpMessage="The file to read"
		)]
		public FileInfo SourceFile{get;set;}

		[Parameter
		(
			Mandatory=true,
			ParameterSetName="Filename",
			Position=0,
			HelpMessage="The file to read"
		)]
		public string Filename{get;set;}

		[Parameter
		(
			Mandatory=false,
			HelpMessage="Options to be used in the pipeline"
		)]
		public ImageOptions ImageOptions{get;set;}

		protected override void ProcessRecord()
		{
			var fileInfo=ResolveFile();

			using(var image=new MagickImage(fileInfo))
			{
				var imageData=new ImageData()
				{
					File=fileInfo,
					Data=image.ToByteArray(),
					ImageOptions=this.ImageOptions
				};

				WriteObject(imageData);
			}
		}

		private FileInfo ResolveFile()
		{
			FileInfo fileInfo=this.SourceFile;

			if(fileInfo==null)
			{
				string path=GetUnresolvedProviderPathFromPSPath(this.Filename);
				fileInfo=new FileInfo(path);
			}

			return fileInfo;
		}
    }
}
