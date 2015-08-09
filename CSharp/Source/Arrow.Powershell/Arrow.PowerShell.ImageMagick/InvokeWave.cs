using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("Invoke","Wave")]
	public class InvokeWave : ImageCmdlet
	{
		public InvokeWave()
		{
			this.Amplitude=25;
			this.Length=150;
		}

		[Parameter(HelpMessage="The amplitude")]
		public double Amplitude{get;set;}

		[Parameter(HelpMessage="The length of the wave")]
		public double Length{get;set;}

		protected override void Apply()
		{
			using(var image=GetMagickImage())
			{
				image.Wave(this.Amplitude,this.Length);

				WriteImage(image);
			}
		}
	}
}
