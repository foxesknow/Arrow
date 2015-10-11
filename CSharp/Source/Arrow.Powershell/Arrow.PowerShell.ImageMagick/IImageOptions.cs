using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.PowerShell.ImageMagick
{
	public interface IImageOptions
	{
		/// <summary>
		/// The color of the border
		/// </summary>
		Color? BorderColor{get;set;}

		/// <summary>
		/// The image quality to use
		/// </summary>
		int? Quality{get;set;}
	}
}
