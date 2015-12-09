using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace Arrow.PowerShell.ImageMagick
{
	public class ImageOptions : IImageOptions
	{
		public ImageOptions()
		{
		}

		internal ImageOptions(IImageOptions rhs)
		{
			this.AlphaColor=rhs.AlphaColor;
			this.AnimationDelay=rhs.AnimationDelay;
			this.AnimationIterations=rhs.AnimationIterations;
			this.BackgroundColor=rhs.BackgroundColor;
			this.BorderColor=rhs.BorderColor;
			this.ColorSpace=rhs.ColorSpace;
			this.Quality=rhs.Quality;
		}

		/// <summary>
		/// The transparent color
		/// </summary>
		public MagickColor AlphaColor{get;set;}

		/// <summary>
		/// Time in 1/100ths of a second which must expire before splaying the next image in an animated sequence.
		/// </summary>
		public int? AnimationDelay{get;set;}

		/// <summary>
		/// Number of iterations to loop an animation (e.g. Netscape loop extension) for
		/// </summary>
		public int? AnimationIterations{get;set;}

		/// <summary>
		/// Anti-alias Postscript and TrueType fonts (default true)
		/// </summary>
		public bool? AntiAlias{get;set;}

		/// <summary>
		/// Background color of the image
		/// </summary>
		public MagickColor BackgroundColor{get;set;}

		/// <summary>
		/// Use black point compensation
		/// </summary>
		public bool? BlackPointCompensation{get;set;}

		/// <summary>
		/// The color of the border
		/// </summary>
		public Color? BorderColor{get;set;}

		/// <summary>
		/// Color space of the image
		/// </summary>
		public ColorSpace? ColorSpace{get;set;}

		/// <summary>
		/// The image quality to use
		/// </summary>
		public int? Quality{get;set;}

		internal void Apply(MagickImage image)
		{
			if(this.AlphaColor!=null) image.AlphaColor=this.AlphaColor;
			if(this.AnimationDelay!=null) image.AnimationDelay=this.AnimationDelay.Value;
			if(this.AnimationIterations!=null) image.AnimationIterations=this.AnimationIterations.Value;
			if(this.AntiAlias!=null) image.AntiAlias=this.AntiAlias.Value;
			if(this.BackgroundColor!=null) image.BackgroundColor=this.BackgroundColor;
			if(this.BlackPointCompensation!=null) image.BlackPointCompensation=this.BlackPointCompensation.Value;
			if(this.BorderColor!=null) image.BorderColor=this.BorderColor.Value;
			if(this.ColorSpace!=null) image.ColorSpace=this.ColorSpace.Value;
			if(this.Quality!=null) image.Quality=this.Quality.Value;
		}
	}
}
