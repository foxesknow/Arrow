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
			this.BorderColor=rhs.BorderColor;
			this.Quality=rhs.Quality;
		}

		/// <summary>
		/// The color of the border
		/// </summary>
		public Color? BorderColor{get;set;}

		/// <summary>
		/// The image quality to use
		/// </summary>
		public int? Quality{get;set;}

		internal void Apply(MagickImage image)
		{
			if(this.BorderColor!=null) image.BorderColor=this.BorderColor.Value;
			if(this.Quality!=null) image.Quality=this.Quality.Value;
		}
	}
}
