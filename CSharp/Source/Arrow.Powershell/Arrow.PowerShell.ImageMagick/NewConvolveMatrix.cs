using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("New","ConvolveMatrix")]
	[OutputType(typeof(ConvolveMatrix))]
	public class NewConvolveMatrix : PSCmdlet
	{
		[Parameter(HelpMessage="The order",Mandatory=true)]
		public int Order{get;set;}

		[Parameter(HelpMessage="The values to initialize the matrix with",Mandatory=false)]
		public double[] Values{get;set;}

		protected override void ProcessRecord()
		{
			if(this.Values==null)
			{
				WriteObject(new ConvolveMatrix(this.Order));
			}
			else
			{
				WriteObject(new ConvolveMatrix(this.Order,this.Values));
			}
		}
	}
}
