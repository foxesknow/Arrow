using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace Arrow.PowerShell.ImageMagick
{
	public interface IImageOptions
	{
		/// <summary>
		/// The transparent color
		/// </summary>
		MagickColor AlphaColor{get;set;}

		/// <summary>
		/// Time in 1/100ths of a second which must expire before splaying the next image in an animated sequence.
		/// </summary>
		int? AnimationDelay{get;set;}

		/// <summary>
		/// Number of iterations to loop an animation (e.g. Netscape loop extension) for
		/// </summary>
		int? AnimationIterations{get;set;}

		/// <summary>
		/// Anti-alias Postscript and TrueType fonts (default true)
		/// </summary>
		bool? AntiAlias{get;set;}

		/// <summary>
		/// Background color of the image
		/// </summary>
		MagickColor BackgroundColor{get;set;}

		/// <summary>
		/// Use black point compensation
		/// </summary>
		bool? BlackPointCompensation{get;set;}

		/// <summary>
		/// The color of the border
		/// </summary>
		Color? BorderColor{get;set;}

		/// <summary>
		/// Color space of the image
		/// </summary>
		ColorSpace? ColorSpace{get;set;}

		/// <summary>
		/// The image quality to use
		/// </summary>
		int? Quality{get;set;}
	}
}
