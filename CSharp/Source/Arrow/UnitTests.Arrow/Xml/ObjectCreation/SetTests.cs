using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Xml.ObjectCreation;
using System.Collections.Frozen;
using System.Xml;

namespace UnitTests.Arrow.Xml.ObjectCreation
{
    [TestFixture]
    public class SetTests : TestBase
    {
        private const string Xml= """
                          <Details>
                            <Numbers>
                                <Number>20</Number>
                                <Number>3</Number>
                                <Number>46</Number>
                            </Numbers>
                          </Details>
            """;

        [Test]
        public void PopulateHashSet()
        {
            var doc = XmlFromString(Xml);

            var factory = InstanceFactory.New();
            var details = factory.Create<Details>(doc.DocumentElement);

            Assert.That(details.Numbers.Count, Is.EqualTo(3));
            Assert.That(details.Numbers.Contains(20), Is.True);
            Assert.That(details.Numbers.Contains(3), Is.True);
            Assert.That(details.Numbers.Contains(46), Is.True);
        }

        [Test]
        public void PopulateReadOnlyHashSet()
        {
            var doc = XmlFromString(Xml);

            var factory = InstanceFactory.New();
            var details = factory.Create<ReadOnlyDetails>(doc.DocumentElement);

            Assert.That(details.Numbers.Count, Is.EqualTo(3));
            Assert.That(details.Numbers.Contains(20), Is.True);
            Assert.That(details.Numbers.Contains(3), Is.True);
            Assert.That(details.Numbers.Contains(46), Is.True);
        }        

        class Details
        {
            public HashSet<int> Numbers{get;} = new();
        }

        class ReadOnlyDetails
        {
            public IReadOnlySet<int> Numbers{get; init;}
        }
    }
}
