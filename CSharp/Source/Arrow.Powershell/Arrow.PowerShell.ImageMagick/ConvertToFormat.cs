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
	[Cmdlet("ConvertTo","Format")]
	public class ConvertToFormat : ImageCmdlet
	{
		[Parameter
		(
			Mandatory=true,
			Position=0,
			HelpMessage="The format to convert to"
		)]
		public MagickFormat Format{get;set;}

		protected override void Apply()
		{
			using(var image=GetMagickImage())
			{
				image.Format=this.Format;

				var imageData=new ImageData()
				{
					File=ChangeExtension(this.Image.File),
					Data=image.ToByteArray()
				};

				WriteObject(imageData);
			}
		}

		private FileInfo ChangeExtension(FileInfo fileInfo)
		{
			string adjusted=Path.ChangeExtension(fileInfo.FullName,this.Format.ToString().ToLower());
			return new FileInfo(adjusted);
		}
	}
}
