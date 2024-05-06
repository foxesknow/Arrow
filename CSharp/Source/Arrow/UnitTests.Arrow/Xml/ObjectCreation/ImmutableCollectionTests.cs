using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Xml.ObjectCreation;
using NUnit.Framework;

namespace UnitTests.Arrow.Xml.ObjectCreation
{
    [TestFixture]
    public class ImmutableCollectionTests : TestBase
    {
        [Test]
        public void ImmutableArray()
        {
            var doc = XmlFromString("""
                            <Details>
                            <Numbers>
                                <Number>10</Number>
                                <Number>1</Number>
                                <Number>99</Number>
                                <Number>10</Number>
                                <Number>81</Number>
                                <Number>50</Number>
                            </Numbers>            
                            </Details>
            """);

            var factory = InstanceFactory.New();
            var details = factory.Create<ImmutableArrayDetails>(doc.DocumentElement);

            Assert.That(details.Numbers.Count, Is.EqualTo(6));
            Assert.That(details.Numbers.Contains(10), Is.True);
            Assert.That(details.Numbers.Contains(1), Is.True);
            Assert.That(details.Numbers.Contains(99), Is.True);
            Assert.That(details.Numbers.Contains(10), Is.True);
            Assert.That(details.Numbers.Contains(81), Is.True);
            Assert.That(details.Numbers.Contains(50), Is.True);
        }

        class ImmutableArrayDetails
        {
            public ImmutableArray<int> Numbers{get; init;}
        }

    }
}
