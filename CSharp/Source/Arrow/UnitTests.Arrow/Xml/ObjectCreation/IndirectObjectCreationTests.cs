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
    public class IndirectObjectCreationTests
    {
        [Test]
        public void CreateAdmin()
        {
            var doc = ResourceLoader.LoadXml("IndirectObjectCreationTests.xml");
            var adminNode = doc.SelectSingleNode("IndirectObjectCreationTests/Admin");

            object rawObject = XmlCreation.Create<object>(adminNode);
            Assert.IsNotNull(rawObject);
            Assert.That(rawObject, Is.TypeOf(typeof(Person)));

            Person person = XmlCreation.Create<Person>(adminNode);
            Assert.IsNotNull(person);
            Assert.That(person.Name, Is.EqualTo("Jack"));
            Assert.That(person.Age, Is.EqualTo(23));
        }

        [Test]
        public void CreateList()
        {
            var doc = ResourceLoader.LoadXml("PeopleList.xml");

            List<Person> people = XmlCreation.CreateList<Person>(doc.SelectNodes("PeopleList/Person"));
            Assert.IsNotNull(people);
            Assert.IsNotEmpty(people);
            Assert.That(people.Count, Is.EqualTo(3));

            Assert.That(people[0].Name, Is.EqualTo("Sun"));
            Assert.That(people[1].Name, Is.EqualTo("Locke"));
            Assert.That(people[2].Name, Is.EqualTo("Jack"));
        }

        [Test]
        public void TestBank()
        {
            var doc = ResourceLoader.LoadXml("IndirectObjectCreationTests.xml");
            var node = doc.SelectSingleNode("IndirectObjectCreationTests/PersonBank");

            PersonBank bank = XmlCreation.Create<PersonBank>(node);
            Assert.IsNotNull(bank);

            Assert.IsNotNull(bank.Primary);
            Assert.That(bank.Primary.Name, Is.EqualTo("Locke"));

            Assert.IsNotNull(bank.Secondary);
            Assert.That(bank.Secondary.Name, Is.EqualTo("Jack"));
        }
    }


}
