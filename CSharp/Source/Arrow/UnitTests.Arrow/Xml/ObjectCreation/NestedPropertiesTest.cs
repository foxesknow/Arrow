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
	public class NestedPropertiesTest
	{
		[Test]
		public void Test()
		{
			var doc=ResourceLoader.LoadXml("NestedProperties.xml");
			var node=doc.SelectSingleNode("NestedProperties/TheOthers");
			
			var factory=new CustomXmlCreation();
			TheOthers others=factory.Create<TheOthers>(node);
			
			Assert.That(others.Leader.Name,Is.EqualTo("Ben"));
			Assert.That(others.Leader.Age,Is.EqualTo(46));
			
			// They should be the same object any more
			Assert.IsFalse(object.ReferenceEquals(others.Medic,others.OriginalMedic));
			
			Assert.That(others.Medic.Name,Is.EqualTo("Juliet"));
			Assert.That(others.Medic.Age,Is.EqualTo(32));
		}
	}
}
