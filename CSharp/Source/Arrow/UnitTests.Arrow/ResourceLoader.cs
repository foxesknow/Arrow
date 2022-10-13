using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace UnitTests.Arrow
{
	public static class ResourceLoader
	{
		public static XmlDocument LoadXml(string name)
		{
			string path="UnitTests.Arrow.Resources.";
			
			using(Stream stream=typeof(ResourceLoader).Assembly.GetManifestResourceStream(path+name))
			{
				XmlDocument doc=new XmlDocument();
				doc.Load(stream);
				
				return doc;
			}
		}
		
		public static Uri MakeUri(string name)
		{
			return new Uri("res://UnitTests.Arrow/UnitTests/Arrow/Resources/"+name);
		}
	}
}
