using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("New","ImageOptions")]
	[OutputType(typeof(ImageOptions))]
	public class NewImageOptions : PSCmdlet, IImageOptions
	{
		[Parameter(HelpMessage="The color for borders")]
		public Color? BorderColor{get;set;}

		[Parameter(HelpMessage="The image quality to use")]
		public int? Quality{get;set;}

		protected override void ProcessRecord()
		{
			var options=new ImageOptions(this);
			WriteObject(options);
		}
	}
}
