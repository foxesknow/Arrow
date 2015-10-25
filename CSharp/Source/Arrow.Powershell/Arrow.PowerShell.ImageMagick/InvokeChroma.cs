using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("Invoke","Chop")]
	public class InvokeChroma : ImageCmdlet
	{
		[Parameter
		(
			Mandatory=true,
			HelpMessage="The chromacity to change"
		)]
		[ValidateSet("Blue","Green","Red","WhitePoint")]
		public string Type{get;set;}

		[Parameter
		(
			Mandatory=true,
			HelpMessage="The X coordinate"
		)]
		public double X{get;set;}

		[Parameter
		(
			Mandatory=true,
			HelpMessage="The Y coordinate"
		)]
		public double Y{get;set;}

		protected override void Apply()
		{
			using(var image=GetMagickImage())
			{
				switch(this.Type)
				{
					case "Red":
						image.ChromaRedPrimary(this.X,this.Y);
						break;

					case "Greed":
						image.ChromaGreenPrimary(this.X,this.Y);
						break;

					case "Blue":
						image.ChromaBluePrimary(this.X,this.Y);
						break;

					case "WhitePoint":
						image.ChromaRedPrimary(this.X,this.Y);
						break;

					default:
						throw new InvalidOperationException("Unknown type: "+this.Type);
				}

				WriteImage(image);
			}
		}
	}
}
