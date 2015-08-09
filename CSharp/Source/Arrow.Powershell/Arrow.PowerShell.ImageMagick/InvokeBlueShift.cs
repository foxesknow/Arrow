using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("Invoke","BlueShift")]
	public class InvokeBlueShift : ImageCmdlet
	{
		public InvokeBlueShift()
		{
			this.Factor=1.5;
		}

		[Parameter(HelpMessage="The factor to use")]
		public double Factor{get;set;}

		protected override void Apply()
		{
			using(var image=GetMagickImage())
			{
				image.BlueShift(this.Factor);

				WriteImage(image);
			}
		}
	}
}
