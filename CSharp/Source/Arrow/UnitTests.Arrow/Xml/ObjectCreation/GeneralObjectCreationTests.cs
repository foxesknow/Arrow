using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Collections;
using Arrow.Xml.ObjectCreation;

using UnitTests.Arrow.Support;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace UnitTests.Arrow.Xml.ObjectCreation
{
    [TestFixture]
    public class GeneralXmlCreationTests
    {
        [Test]
        public void TestBasicPerson()
        {
            var doc = ResourceLoader.LoadXml("GeneralObjectCreationTests.xml");
            var basicPersonNode = doc.SelectSingleNode("GeneralObjectCreationTests/BasicPerson");

            object p = XmlCreation.Create<object>(basicPersonNode);
            Assert.IsNotNull(p);
            Assert.That(p, Is.InstanceOf<Person>());

            // We didn't set any properties so they should have their default values
            Person person = (Person)p;
            Assert.IsNull(person.Name);
            Assert.IsTrue(person.Age == 0);
        }

        [Test]
        public void TestNormalPerson()
        {
            var doc = ResourceLoader.LoadXml("GeneralObjectCreationTests.xml");
            var normalPersonNode = doc.SelectSingleNode("GeneralObjectCreationTests/NormalPerson");

            object p = XmlCreation.Create<object>(normalPersonNode);
            Assert.IsNotNull(p);
            Assert.That(p, Is.InstanceOf<Person>());

            Person person = (Person)p;
            Assert.IsTrue(person.Name == "Sean");
            Assert.IsTrue(person.Age == 35);
        }

        [Test]
        public void TestNormalPersonWithConversion()
        {
            var doc = ResourceLoader.LoadXml("GeneralObjectCreationTests.xml");
            var normalPersonNode = doc.SelectSingleNode("GeneralObjectCreationTests/NormalPersonConversion");

            object p = XmlCreation.Create<object>(normalPersonNode);
            Assert.IsNotNull(p);
            Assert.That(p, Is.InstanceOf<Person>());

            Person person = (Person)p;
            Assert.IsTrue(person.Name == "Sean");
            Assert.IsTrue(person.Age == 35);
        }

        [Test]
        public void TestNormalPersonCall()
        {
            var doc = ResourceLoader.LoadXml("GeneralObjectCreationTests.xml");
            var normalPersonNode = doc.SelectSingleNode("GeneralObjectCreationTests/NormalPersonCall");

            object p = XmlCreation.Create<object>(normalPersonNode);
            Assert.IsNotNull(p);
            Assert.That(p, Is.InstanceOf<Person>());

            Person person = (Person)p;
            Assert.IsTrue(person.Name == "Sean");
            Assert.IsTrue(person.Age == 42);
        }

        [Test]
        public void TestSimpleEnum()
        {
            var doc = ResourceLoader.LoadXml("GeneralObjectCreationTests.xml");
            var node = doc.SelectSingleNode("GeneralObjectCreationTests/FileModeRead");

            Mode mode = XmlCreation.Create<Mode>(node);
            Assert.That(mode, Is.EqualTo(Mode.Read));
        }

        [Test]
        public void TestFlagEnum()
        {
            var doc = ResourceLoader.LoadXml("GeneralObjectCreationTests.xml");
            var node = doc.SelectSingleNode("GeneralObjectCreationTests/FileModeFlags");

            Mode mode = XmlCreation.Create<Mode>(node);
            Assert.That(mode, Is.EqualTo(Mode.Read | Mode.Execute));
        }

        [Test]
        public void TestAllFeatures()
        {
            var doc = ResourceLoader.LoadXml("GeneralObjectCreationTests.xml");
            var node = doc.SelectSingleNode("GeneralObjectCreationTests/Foo");

            Foo foo = XmlCreation.Create<Foo>(node);
            ValidateFoo(foo);
        }

        [Test]
        public void TestApply()
        {
            var doc = ResourceLoader.LoadXml("GeneralObjectCreationTests.xml");
            var node = doc.SelectSingleNode("GeneralObjectCreationTests/Foo");

            Foo foo = new Foo("Sean", true);
            XmlCreation.Apply(foo, node);
            ValidateFoo(foo);
        }

        [Test]
        public void TestCreateList()
        {
            var doc = ResourceLoader.LoadXml("GeneralObjectCreationTests.xml");
            var nodes = doc.SelectNodes("GeneralObjectCreationTests/Names/Name");

            List<string> names = XmlCreation.CreateList<string>(nodes);
            Assert.IsNotNull(names);
            Assert.IsTrue(names.Count == 4);
        }

        private void ValidateFoo(Foo foo)
        {
            Assert.IsNotNull(foo);
            Assert.IsTrue(foo.Username == "Sean");
            Assert.IsTrue(foo.Enabled);

            Assert.IsTrue(foo.Offset.HasValue);
            Assert.IsTrue(foo.Offset.Value == 3);

            Assert.IsNotNull(foo.Basic);
            Assert.IsTrue(foo.Basic is BasicBar);

            Assert.IsFalse(foo.Allow.HasValue);

            Assert.IsTrue(foo.When == new DateTime(2008, 8, 8));

            Assert.IsTrue(foo.Numbers.Count == 6);
            Assert.IsTrue(foo.Numbers.Contains(4));
            Assert.IsTrue(foo.Numbers.Contains(8));
            Assert.IsTrue(foo.Numbers.Contains(15));
            Assert.IsTrue(foo.Numbers.Contains(16));
            Assert.IsTrue(foo.Numbers.Contains(23));
            Assert.IsTrue(foo.Numbers.Contains(42));

            Assert.IsTrue(foo.Basics.Count == 2);
            Assert.IsTrue(foo.Basics.ContainsKey("foo"));
            Assert.IsTrue(foo.Basics.ContainsKey("bar"));

            Assert.IsTrue(foo.Basics["foo"].GetType() == typeof(BasicFoo));
            Assert.IsTrue(foo.Basics["bar"].GetType() == typeof(BasicBar));

            Assert.IsTrue(foo.Ages.Count == 2);
            Assert.IsTrue(foo.Ages.ContainsKey("Sean"));
            Assert.IsTrue(foo.Ages["Sean"] == 35);
            Assert.IsTrue(foo.Ages.ContainsKey("Fred"));
            Assert.IsTrue(foo.Ages["Fred"] == 46);
        }

        [Test]
        public void TestInitialize()
        {
            var doc = ResourceLoader.LoadXml("GeneralObjectCreationTests.xml");
            var node = doc.SelectSingleNode("GeneralObjectCreationTests/InitTest");

            InitializeTester t = XmlCreation.Create<object>(node) as InitializeTester;

            Assert.IsTrue(t.m_BeginInitCalled);
            Assert.IsTrue(t.m_EndInitCalled);
        }

        [Test]
        public void TestXmlAssignment()
        {
            // If a property is an var it will be
            // assigned the node directly instead of parsing the
            // node. This test makes sure this is the case
            var doc = ResourceLoader.LoadXml("GeneralObjectCreationTests.xml");
            var node = doc.SelectSingleNode("GeneralObjectCreationTests/XmlAssignment");

            XmlAssignment x = XmlCreation.Create<XmlAssignment>(node);
            Assert.IsNotNull(x);
            Assert.That(x.Age, Is.EqualTo(31));

            // Validate the xml
            var configNode = x.Config;
            Assert.IsNotNull(configNode);
            Assert.IsNotNull(configNode.SelectSingleNode("Jack"));
            Assert.IsNotNull(configNode.SelectSingleNode("Locke"));
        }

        [Test]
        public void TestDelayedCreate()
        {
            var doc = ResourceLoader.LoadXml("Jack.xml");

            DelayedCreator creator = XmlCreation.DelayedCreate<object>(doc.DocumentElement);
            Assert.IsNotNull(creator);
            Assert.That(creator.UnderlyingType, Is.EqualTo(typeof(Person)));

            Person p = creator.Create<Person>();
            Assert.IsNotNull(p);
            Assert.That(p.Name, Is.EqualTo("Jack"));
            Assert.That(p.Age, Is.EqualTo(23));
        }

        [Test]
        public void TestSimpleDictionary()
        {
            var doc = ResourceLoader.LoadXml("GeneralObjectCreationTests.xml");
            var node = doc.SelectSingleNode("GeneralObjectCreationTests/Ages");

            CustomXmlCreation factory = new CustomXmlCreation();
            Dictionary<string, int> ages = new Dictionary<string, int>();
            factory.PopulateDictionary(ages, node.SelectNodes("*|@*"));

            Assert.That(ages.Count, Is.EqualTo(3));
            Assert.That(ages["Sean"], Is.EqualTo(35));
            Assert.That(ages["Jon"], Is.EqualTo(34));
            Assert.That(ages["Rob"], Is.EqualTo(27));
        }

        [Test]
        public void TestKeyValuePair()
        {
            var doc = ResourceLoader.LoadXml("GeneralObjectCreationTests.xml");
            var node = doc.SelectSingleNode("GeneralObjectCreationTests/Codes");

            CustomXmlCreation factory = new CustomXmlCreation();
            Dictionary<string, int> codes = new Dictionary<string, int>();

            factory.PopulateKeyValuePair(codes, node.SelectNodes("*"));
            Assert.That(codes.Count, Is.EqualTo(5));
            Assert.That(codes["A"], Is.EqualTo(1));
            Assert.That(codes["B"], Is.EqualTo(2));
            Assert.That(codes["C"], Is.EqualTo(3));
            Assert.That(codes["D"], Is.EqualTo(4));
            Assert.That(codes["E"], Is.EqualTo(5));
        }

        [Test]
        public void TestMultiDictionary()
        {
            var doc = ResourceLoader.LoadXml("GeneralObjectCreationTests.xml");
            var nodes = doc.SelectNodes("GeneralObjectCreationTests/Scores/*");

            CustomXmlCreation factory = new CustomXmlCreation();
            MultiDictionary<string, int> scores = new MultiDictionary<string, int>();
            factory.PopulateDictionary(scores, nodes);

            Assert.That(scores.Count, Is.EqualTo(5));
            Assert.That(scores.KeyCount, Is.EqualTo(2));

            Assert.That(scores.ValuesFor("Sean"), Has.Member(8));
            Assert.That(scores.ValuesFor("Sean"), Has.Member(10));
            Assert.That(scores.ValuesFor("Jack"), Has.Member(1));
            Assert.That(scores.ValuesFor("Jack"), Has.Member(2));
            Assert.That(scores.ValuesFor("Jack"), Has.Member(13));
        }

        [Test]
        public void TestAnyTypeDictionary()
        {
            var doc = ResourceLoader.LoadXml("GeneralObjectCreationTests.xml");
            var nodes = doc.SelectNodes("GeneralObjectCreationTests/NamedBag/*");

            CustomXmlCreation factory = new CustomXmlCreation();
            Dictionary<string, object> bag = new Dictionary<string, object>();
            factory.PopulateDictionary(bag, nodes);

            Assert.That(bag.Count, Is.EqualTo(2));

            object number = bag["Number"];
            Assert.IsNotNull(number);
            Assert.That(number, Is.TypeOf(typeof(int)));
            Assert.That(number, Is.EqualTo(42));

            object text = bag["Text"];
            Assert.IsNotNull(text);
            Assert.That(text, Is.TypeOf(typeof(string)));
            Assert.That(text, Is.EqualTo("hello"));
        }

        [Test]
        public void FromAttribute()
        {
            var doc = ResourceLoader.LoadXml("GeneralObjectCreationTests.xml");
            var node = doc.SelectSingleNode("GeneralObjectCreationTests/FromAttribute");
            var ageAttr = node.Attributes["age"];

            CustomXmlCreation factory = new CustomXmlCreation();
            int age = factory.Create<int>(ageAttr);
            Assert.That(age, Is.EqualTo(10));
        }

        [Test]
        public void ApplyNodeAttributes()
        {
            var doc = ResourceLoader.LoadXml("GeneralObjectCreationTests.xml");
            var node = doc.SelectSingleNode("GeneralObjectCreationTests/ElementPerson");

            CustomXmlCreation factory = new CustomXmlCreation();
            Person p = new Person();
            factory.ApplyNodeAttributes(p, node);

            Assert.That(p.Name, Is.EqualTo("Sun"));
            Assert.That(p.Age, Is.EqualTo(41));
        }

        [Test]
        public void DisableElementExpansion()
        {
            var doc = ResourceLoader.LoadXml("GeneralObjectCreationTests.xml");
            var node = doc.SelectSingleNode("GeneralObjectCreationTests/NoExpandPerson");

            CustomXmlCreation factory = new CustomXmlCreation();
            Person p = factory.Create<Person>(node);

            Assert.That(p.Name, Is.EqualTo("${DateTime:Now}"));
            Assert.That(p.Age, Is.EqualTo(10));
        }

        [Test]
        public void UnknownVariableLookup()
        {
            var doc = ResourceLoader.LoadXml("GeneralObjectCreationTests.xml");
            var node = doc.SelectSingleNode("GeneralObjectCreationTests/VariableLookupPerson");

            Func<string, object> lookup = name =>
            {
                if(name == "default-name") return "Hurley";
                if(name == "default-age") return 28;
                return null;
            };

            CustomXmlCreation factory = new CustomXmlCreation(lookup);
            Person p = factory.Create<Person>(node);

            Assert.That(p.Name, Is.EqualTo("Hurley"));
            Assert.That(p.Age, Is.EqualTo(28));
        }
    }
}
