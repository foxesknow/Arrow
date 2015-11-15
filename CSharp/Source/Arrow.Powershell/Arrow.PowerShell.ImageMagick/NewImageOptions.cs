using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("New","ImageOptions")]
	[OutputType(typeof(ImageOptions))]
	public class NewImageOptions : PSCmdlet, IImageOptions
	{
		[Parameter(HelpMessage="The transparent color")]
		public MagickColor AlphaColor{get;set;}

		[Parameter(HelpMessage="Time in 1/100ths of a second which must expire before splaying the next image in an animated sequence")]
		public int? AnimationDelay{get;set;}

		[Parameter(HelpMessage="Number of iterations to loop an animation (e.g. Netscape loop extension) for")]
		public int? AnimationIterations{get;set;}

		[Parameter(HelpMessage="Anti-alias Postscript and TrueType fonts (default true)")]
		public bool? AntiAlias{get;set;}

		[Parameter(HelpMessage="Background color of the image")]
		public MagickColor BackgroundColor{get;set;}

		[Parameter(HelpMessage="Use black point compensation")]
		public bool? BlackPointCompensation{get;set;}

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
