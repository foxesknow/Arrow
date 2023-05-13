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
    public class ICollectionTests
    {
        [Test]
        public void Test()
        {
            var doc = ResourceLoader.LoadXml("HashCollection.xml");

            var factory = new CustomXmlCreation();
            HashSetData hashSetData = factory.Create<HashSetData>(doc.DocumentElement);

            Assert.That(hashSetData.Data.Count, Is.EqualTo(3));
            Assert.That(hashSetData.Data, Has.Member(1));
            Assert.That(hashSetData.Data, Has.Member(2));
            Assert.That(hashSetData.Data, Has.Member(3));
        }
    }


}
