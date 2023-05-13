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
    public class UriCreationTests
    {
        [Test]
        public void CreateFromSelect()
        {
            Uri uri = new Uri(@"res://UnitTests.Arrow/UnitTests/Arrow/Resources/People.xml?select=/People/Hunter");

            var factory = new CustomXmlCreation();
            object obj = factory.UriCreate(uri);
            Assert.IsNotNull(obj);

            Person p = (Person)obj;
            Assert.That(p.Name, Is.EqualTo("Locke"));
            Assert.That(p.Age, Is.EqualTo(52));
        }

        [Test]
        public void Create()
        {
            Uri uri = new Uri(@"res://UnitTests.Arrow/UnitTests/Arrow/Resources/Jack.xml");

            var factory = new CustomXmlCreation();
            Person p = factory.UriCreate<Person>(uri);

            Assert.IsNotNull(p);
            Assert.That(p.Name, Is.EqualTo("Jack"));
            Assert.That(p.Age, Is.EqualTo(23));
        }

        [Test]
        public void CreateList()
        {
            Uri uri = new Uri(@"res://UnitTests.Arrow/UnitTests/Arrow/Resources/PeopleList.xml");

            var factory = new CustomXmlCreation();
            List<Person> people = factory.UriCreateList<Person>(uri);

            Assert.IsNotNull(people);
            Assert.That(people, Has.Count.EqualTo(3));
        }

        [Test]
        public void CreateDictionary()
        {
            Uri uri = new Uri(@"res://UnitTests.Arrow/UnitTests/Arrow/Resources/ObjectCreationTests.xml?select=/root/Ages/*");

            var factory = new CustomXmlCreation();
            Dictionary<string, int> ages = new Dictionary<string, int>();
            factory.UriPopulateDictionary(ages, uri);

            Assert.That(ages, Has.Count.EqualTo(3));
            Assert.That(ages["Sean"], Is.EqualTo(35));
            Assert.That(ages["Jon"], Is.EqualTo(34));
            Assert.That(ages["Rob"], Is.EqualTo(27));
        }
    }
}
