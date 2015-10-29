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
	[Cmdlet("New","MagicColor")]
	[OutputType(typeof(MagickColor))]
	public class NewMagickColor : PSCmdlet
	{
		[Parameter(Mandatory=true,ParameterSetName="Color")]
		public Color Color{get;set;}


		[Parameter(Mandatory=true,ParameterSetName="StringColor")]
		public string StringColor{get;set;}


		[Parameter(Mandatory=true,ParameterSetName="RGB")]
		[ValidateRange(0,65535)]
		public int Red{get;set;}

		[Parameter(Mandatory=true,ParameterSetName="RGB")]
		[ValidateRange(0,65535)]
		public int Green{get;set;}

		[Parameter(Mandatory=true,ParameterSetName="RGB")]
		[ValidateRange(0,65535)]
		public int Blue{get;set;}

		[Parameter(Mandatory=false,ParameterSetName="RGB")]
		[Parameter(Mandatory=true,ParameterSetName="CMYK")]
		[ValidateRange(0,65535)]
		public int? Alpha{get;set;}


		[Parameter(Mandatory=true,ParameterSetName="CMYK")]
		[ValidateRange(0,65535)]
		public int Cyan{get;set;}

		[Parameter(Mandatory=true,ParameterSetName="CMYK")]
		[ValidateRange(0,65535)]
		public int Magenta{get;set;}

		[Parameter(Mandatory=true,ParameterSetName="CMYK")]
		[ValidateRange(0,65535)]
		public int Yellow{get;set;}

		[Parameter(Mandatory=true,ParameterSetName="CMYK")]
		[ValidateRange(0,65535)]
		public int Black{get;set;}

		protected override void ProcessRecord()
		{
			MagickColor m=null;

			switch(this.ParameterSetName)
			{
				case "Color":
					m=new MagickColor(this.Color);
					break;

				case "StringColor":
					m=new MagickColor(this.StringColor);
					break;

				case "RGB":
					if(this.Alpha.HasValue)
					{
						m=new MagickColor((ushort)this.Red,(ushort)this.Green,(ushort)this.Blue,(ushort)this.Alpha.Value);
					}
					else
					{
						m=new MagickColor((ushort)this.Red,(ushort)this.Green,(ushort)this.Blue);
					}
					break;

				case "CMYK":
					m=new MagickColor((ushort)this.Cyan,(ushort)this.Magenta,(ushort)this.Yellow,(ushort)this.Black,(ushort)this.Alpha.Value);
					break;

				default:
					throw new InvalidOperationException("Unknown parameter set: "+this.ParameterSetName);
			}

			WriteObject(m);
		}
	}
}
