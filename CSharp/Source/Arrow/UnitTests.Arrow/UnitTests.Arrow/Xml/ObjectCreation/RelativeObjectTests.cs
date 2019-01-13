using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

using Arrow.Xml.ObjectCreation;
using Arrow.Storage;

using UnitTests.Arrow.Support;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace UnitTests.Arrow.Xml.ObjectCreation
{
	[TestFixture]
	public class RelativeObjectTests
	{
		[Test]
		public void CreateObject()
		{
			Uri uri=new Uri(@"res://UnitTests.Arrow/UnitTests/Arrow/Resources/RelativeObjectCreation.xml");
			
			using(Stream stream=StorageManager.Get(uri).OpenRead())
			{
				var doc=new XmlDocument();
				doc.Load(stream);
				
				var factory=new CustomXmlCreation();
				
				var node=doc.SelectSingleNode("root/User");
				Person p=factory.Create<Person>(node,uri);
				
				Assert.IsNotNull(p);
			}
		}
		
		[Test]
		public void CreateList()
		{
			Uri uri=new Uri(@"res://UnitTests.Arrow/UnitTests/Arrow/Resources/RelativeObjectCreation.xml");
			
			using(Stream stream=StorageManager.Get(uri).OpenRead())
			{
				var doc=new XmlDocument();
				doc.Load(stream);
				
				var factory=new CustomXmlCreation();
				
				var nodes=doc.SelectNodes("root/People/Person");
				List<Person> people=factory.CreateList<Person>(nodes,uri);
				
				Assert.IsNotNull(people);
				Assert.That(people,Has.Count.EqualTo(3));
			}
		}
		
		[Test]
		public void NoBaseSpecified()
		{
            Assert.Throws<UriFormatException>(() =>
            {
			    Uri uri=new Uri(@"res://UnitTests.Arrow/UnitTests/Arrow/Resources/RelativeObjectCreation.xml");
			
			    using(Stream stream=StorageManager.Get(uri).OpenRead())
			    {
				    var doc=new XmlDocument();
				    doc.Load(stream);
				
				    var factory=new CustomXmlCreation();
				
				    // This will fail because the document specifies a 
				    // relative url and we've not specifed the base
				    var node=doc.SelectSingleNode("root/WontWork");
				    Person p=factory.Create<Person>(node,null);
				
				    Assert.IsNotNull(p);
				    Assert.That(p.Name,Is.EqualTo("Jack"));
				    Assert.That(p.Age,Is.EqualTo(12));
			    }
            });
		}
	}
}
