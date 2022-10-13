using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

using Arrow.Storage;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace UnitTests.Arrow.Storage
{
	[TestFixture]
	public class XmlStorageResolverTests
	{
		[Test]
		public void LoadStylesheet()
		{
			// The sytlesheet we're loading has a xsl:include element which will be automatically resolved
			Uri uri=new Uri("res://UnitTests.Arrow/UnitTests/Arrow/Resources/StorageStylesheet.xml");
			
			var resolver=new XmlStorageResolver();
			
			XslCompiledTransform transform=new XslCompiledTransform();
			transform.Load(uri.ToString(),null,resolver);
		}
		
		[Test]
		public void LoadDocument()
		{
			XmlDocument doc=new XmlDocument();
			doc.XmlResolver=new XmlStorageResolver();
			
			string uri="res://UnitTests.Arrow/UnitTests/Arrow/Resources/People.xml";
			doc.Load(uri);
			
			Assert.That(doc.DocumentElement.Name,Is.EqualTo("People"));
		}	
	}
}
