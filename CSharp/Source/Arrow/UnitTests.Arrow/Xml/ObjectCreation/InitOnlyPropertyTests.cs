using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using Arrow.Xml.ObjectCreation;

using UnitTests.Arrow.Support;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace UnitTests.Arrow.Xml.ObjectCreation
{
    [TestFixture]
    public class InitOnlyPropertyTests
    {
        [Test]
        public void SetInitOnly()
        {
            var xml = @"
                <Location>
                    <Name>Jack</Name>
                    <City>London</City>
                </Location>
            ";

            var doc = new XmlDocument();
            doc.LoadXml(xml);

            var factory = InstanceFactory.New();
            var location = factory.Create<Location>(doc.DocumentElement);

            Assert.That(location.Name, Is.EqualTo("Jack"));
            Assert.That(location.City, Is.EqualTo("London"));
        }

        class Location
        {
            public string Name{get; init;}
            public string City{get; init;}
        }
    }
}
