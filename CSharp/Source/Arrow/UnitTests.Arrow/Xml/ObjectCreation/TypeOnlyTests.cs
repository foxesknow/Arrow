using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Xml.ObjectCreation;

using UnitTests.Arrow.Support;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace UnitTests.Arrow.Xml.ObjectCreation
{
	[TestFixture]
	public class TypeOnlyTests
	{
		[Test]
		public void CreateFirst()
		{
			var doc=ResourceLoader.LoadXml("TypeOnlyTests.xml");
			var node=doc.SelectSingleNode("root/FirstOnly");

			TypeHolder holder=XmlCreation.Create<TypeHolder>(node);
			Assert.That(holder,Is.Not.Null);

			Assert.That(holder.First,Is.Not.Null);
			Assert.That(holder.First,Is.EqualTo(typeof(Person)));

			Assert.That(holder.Second,Is.Null);
		}

		[Test]
		public void CreateFirstAndSecond()
		{
			var doc=ResourceLoader.LoadXml("TypeOnlyTests.xml");
			var node=doc.SelectSingleNode("root/FirstAndSecond");

			TypeHolder holder=XmlCreation.Create<TypeHolder>(node);
			Assert.That(holder,Is.Not.Null);

			Assert.That(holder.First,Is.Not.Null);
			Assert.That(holder.First,Is.EqualTo(typeof(Person)));

			Assert.That(holder.Second,Is.Not.Null);
			Assert.That(holder.Second,Is.EqualTo(typeof(System.Collections.ArrayList)));
		}

		[Test]
		public void ListOfTypeHolder()
		{
			var doc=ResourceLoader.LoadXml("TypeOnlyTests.xml");
			var nodes=doc.SelectNodes("root/ListOfTypeHolder/TypeHolder");

			var types=XmlCreation.CreateList<TypeHolder>(nodes);
			
			Assert.That(types,Is.Not.Null);
			Assert.That(types,Has.Count.EqualTo(2));

			Assert.That(types[0].First,Is.EqualTo(typeof(Person)));
			Assert.That(types[0].Second,Is.EqualTo(typeof(TheOthers)));

			Assert.That(types[1].First,Is.EqualTo(typeof(TheOthers)));
			Assert.That(types[1].Second,Is.EqualTo(typeof(PersonBank)));
		}

		[Test]
		public void ListOfType()
		{
			var doc=ResourceLoader.LoadXml("TypeOnlyTests.xml");
			var nodes=doc.SelectNodes("root/ListOfType/Type");

			var types=XmlCreation.CreateList<Type>(nodes);
			
			Assert.That(types,Is.Not.Null);
			Assert.That(types,Has.Count.EqualTo(3));
		}
	}
}
