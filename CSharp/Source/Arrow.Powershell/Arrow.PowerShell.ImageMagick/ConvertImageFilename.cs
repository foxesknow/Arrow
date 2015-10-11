using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("Convert","ImageFilename")]
	public class ConvertImageFilename : ImageCmdlet
	{
		[Parameter(HelpMessage="A script to generate the directory name")]
		public ScriptBlock Filename{get;set;}

		[Parameter(HelpMessage="A script to generate a filename")]
		public ScriptBlock Directory{get;set;}

		protected override void Apply()
		{
			var fileInfo=this.Image.File;

			string directory=fileInfo.DirectoryName;
			string filename=Path.GetFileNameWithoutExtension(fileInfo.FullName);
			string extension=fileInfo.Extension;

			string newFilename=RunScript(this.Filename,filename);
			
			string newDirectory=RunScript(this.Directory,directory);
			newDirectory=GetUnresolvedProviderPathFromPSPath(newDirectory);

			string name=Path.Combine(newDirectory,newFilename);
			name=Path.ChangeExtension(name,extension);

			var imageData=new ImageData()
			{
				File=new FileInfo(name),
				Data=this.Image.Data,
				ImageOptions=this.Image.ImageOptions
			};

			WriteObject(imageData);
		}

		private string RunScript(ScriptBlock block, string variable)
		{
			if(block==null) return variable;

			var variables=new List<PSVariable>()
			{
				new PSVariable("_",variable),
				new PSVariable("psitem",variable)
			};

			var result=block.InvokeWithContext(null,variables);
			var newValue=result.FirstOrDefault();

			return newValue==null ? variable : newValue.BaseObject.ToString();
		}
	}
}
