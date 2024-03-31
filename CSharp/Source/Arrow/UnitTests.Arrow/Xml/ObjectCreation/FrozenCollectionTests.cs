using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Arrow.Xml.ObjectCreation;
using NUnit.Framework;

namespace UnitTests.Arrow.Xml.ObjectCreation
{
    [TestFixture]
    public class FrozenCollectionTests : TestBase
    {
        [Test]
        public void FrozenDictionary()
        {
            var doc = XmlFromString("""
                          <Details>
                            <Ages>
                                <Name key='Jack' value='34' />
                                <Name key='Kate' value='31' />
                            </Ages>
                          </Details>
            """);

            var factory = InstanceFactory.New();
            var details = factory.Create<DetailsDictionary>(doc.DocumentElement);

            Assert.That(details.Ages.Count, Is.EqualTo(2));
            Assert.That(details.Ages["Jack"], Is.EqualTo(34));
            Assert.That(details.Ages["Kate"], Is.EqualTo(31));
        }

        [Test]
        public void FrozenSet()
        {
            var doc = XmlFromString("""
                          <Details>
                            <Numbers>
                                <Number>10</Number>
                                <Number>1</Number>
                                <Number>99</Number>
                                <Number>10</Number>
                            </Numbers>            
                          </Details>
            """);

            var factory = InstanceFactory.New();
            var details = factory.Create<DetailsSet>(doc.DocumentElement);

            Assert.That(details.Numbers.Count, Is.EqualTo(3));
            Assert.That(details.Numbers.Contains(10), Is.True);
            Assert.That(details.Numbers.Contains(1), Is.True);
            Assert.That(details.Numbers.Contains(99), Is.True);
        }

        class DetailsDictionary
        {
            public FrozenDictionary<string, int> Ages{get; init;}
        }

        class DetailsSet
        {
            public FrozenSet<int> Numbers{get; init;}
        }
    }
}
