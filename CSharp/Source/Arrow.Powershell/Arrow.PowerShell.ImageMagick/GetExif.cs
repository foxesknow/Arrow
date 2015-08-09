using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet(VerbsCommon.Get,"Exif")]
	public class GetExif : ImageCmdlet
	{
		protected override void Apply()
		{
			using(var image=GetMagickImage())
			{
				var profile=image.GetExifProfile();

				var @object=new PSObject();

				foreach(var value in profile.Values)
				{
					var note=new PSNoteProperty(value.Tag.ToString(),value.Value);
					@object.Properties.Add(note);
				}

				// Add the image in so that we can keep on piping
				@object.Properties.Add(new PSNoteProperty("Image",this.Image));

				WriteObject(@object);
			}
		}
	}
}
