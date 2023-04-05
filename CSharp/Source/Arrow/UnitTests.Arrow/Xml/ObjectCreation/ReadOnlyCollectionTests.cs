using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Xml.ObjectCreation;

using UnitTests.Arrow.Support;

using NUnit.Framework;
using System.Xml;

namespace UnitTests.Arrow.Xml.ObjectCreation
{
    [TestFixture]
    public class ReadOnlyCollectionTests
    {
        [Test]
        public void MakeLocations()
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
        public void MakeIsland()
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

        private XmlDocument XmlFromString(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            return doc;
        }

        class Locations
        {
            public IReadOnlyList<string> Names{get; init;}
        }

        class Island
        {
            public string Name{get; init;}
            public IReadOnlyDictionary<string, int> Ages{get; init;}
        }
    }
}
