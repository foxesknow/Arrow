using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("Invoke","Flip")]
	public class InvokeFlip : ImageCmdlet
	{
		private bool m_Horizontal;
		private bool m_Vertical;

		[Parameter(HelpMessage="Flip horizontally")]
		public SwitchParameter Horizontal
		{
			get{return m_Horizontal;}
			set{m_Horizontal=value;}
		}

		[Parameter(HelpMessage="Flip vertically")]
		public SwitchParameter Vertical
		{
			get{return m_Vertical;}
			set{m_Vertical=value;}
		}

		protected override void Apply()
		{
 			using(var image=GetMagickImage())
			{
				if(m_Horizontal) image.Flop();
				if(m_Vertical) image.Flip();

				WriteImage(image);
			}
		}
	}
}
