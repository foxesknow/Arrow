using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace Arrow.PowerShell.ImageMagick
{
	[Cmdlet("Get","ImageOptions")]
	public class GetImageOptions : ImageCmdlet
	{
		protected override void Apply()
		{
			using(var image=GetMagickImage())
			{
				var options=new PSObject();

				var properties=typeof(MagickImage).GetProperties(BindingFlags.Public|BindingFlags.Instance);
				foreach(var property in properties)
				{
					if(property.CanRead)
					{
						var name=property.Name;
						var value=property.GetValue(image);

						options.Properties.Add(new PSNoteProperty(name,value));
					}
				}

				WriteObject(options);
			}
		}
	}
}
