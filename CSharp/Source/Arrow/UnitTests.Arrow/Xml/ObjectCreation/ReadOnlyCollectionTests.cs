using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using Arrow.Xml.ObjectCreation;

using UnitTests.Arrow.Support;

using NUnit.Framework;


namespace UnitTests.Arrow.Xml.ObjectCreation
{
    [TestFixture]
    public class ReadOnlyCollectionTests : TestBase
    {
        [Test]
        public void List()
        {
            var doc = XmlFromString(
            @"
                <Locations>
                    <Names>
                        <Name>London</Name>
                        <Name>Paris</Name>
                    </Names>
                </Locations>
            ");

            var factory = InstanceFactory.New();
            var locations = factory.Create<Locations>(doc.DocumentElement);

            Assert.That(locations.Names.Count, Is.EqualTo(2));
            Assert.That(locations.Names[0], Is.EqualTo("London"));
            Assert.That(locations.Names[1], Is.EqualTo("Paris"));
        }

        [Test]
        public void Dictionary()
        {
            var doc = XmlFromString(
            @"
                <Island>
                    <Name>Lundy</Name>
                    <Ages>
                        <Name key='Jack' value='34' />
                        <Name key='Kate' value='31' />
                    </Ages>
                </Island>
            ");

            var factory = InstanceFactory.New();
            var island = factory.Create<Island>(doc.DocumentElement);

            Assert.That(island.Name, Is.EqualTo("Lundy"));
            Assert.That(island.Ages.Count, Is.EqualTo(2));
            Assert.That(island.Ages["Jack"], Is.EqualTo(34));
            Assert.That(island.Ages["Kate"], Is.EqualTo(31));
        }

        [Test]
        public void Set()
        {
            var doc = XmlFromString(
            @"
                <Data>
                    <Numbers>
                        <Number>10</Number>
                        <Number>2</Number>
                        <Number>9</Number>
                        <Number>7</Number>
                        <Number>9</Number>
                    </Numbers>
                </Data>
            ");

            var factory = InstanceFactory.New();
            var data = factory.Create<Data>(doc.DocumentElement);

            Assert.That(data.Numbers.Count, Is.EqualTo(4));
            Assert.That(data.Numbers.Contains(10), Is.True);
            Assert.That(data.Numbers.Contains(2), Is.True);
            Assert.That(data.Numbers.Contains(9), Is.True);
            Assert.That(data.Numbers.Contains(7), Is.True);
        }

        class Locations
        {
            public IReadOnlyList<string> Names { get; init; }
        }

        class Island
        {
            public string Name { get; init; }
            public IReadOnlyDictionary<string, int> Ages { get; init; }
        }

        class Data
        {
            public IReadOnlySet<int> Numbers{get; init;}
        }
    }
}
