using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Xml.ObjectCreation;

using UnitTests.Arrow.Support;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace UnitTests.Arrow.Xml.ObjectCreation
{
	[TestFixture]
	public class NamespaceTypeTests
	{
		[Test]
		public void TestPersonIndirect()
		{
			var doc=ResourceLoader.LoadXml("NamespaceTypeTests.xml");
			var personNode=doc.SelectSingleNode("Tests/Person");
			
			object obj=XmlCreation.Create<object>(personNode);
			Assert.IsNotNull(obj);
			Assert.That(obj,Is.TypeOf(typeof(Person)));
			
			Person p=(Person)obj;
			Assert.That(p.Name,Is.EqualTo("Sean"));
			Assert.That(p.Age,Is.EqualTo(35));
		}
		
		[Test]
		public void TestInclude()
		{
			var doc=ResourceLoader.LoadXml("NamespaceTypeTests.xml");
			var personNode=doc.SelectSingleNode("Tests/Passenger");
			
			Person p=XmlCreation.Create<Person>(personNode);
			Assert.IsNotNull(p);
			Assert.That(p.Name,Is.EqualTo("Jack"));
			Assert.That(p.Age,Is.EqualTo(23));
		}
	}
}
