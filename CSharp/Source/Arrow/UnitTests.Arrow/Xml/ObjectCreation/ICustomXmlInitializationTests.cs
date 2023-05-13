using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using Arrow.Xml.ObjectCreation;

using UnitTests.Arrow.Support;

using NUnit.Framework;
using NUnit.Framework.Constraints;


namespace UnitTests.Arrow.Xml.ObjectCreation
{
    [TestFixture]
    public class ICustomXmlInitializationTests
    {
        [Test]
        public void InitializeObject()
        {
            var doc = ResourceLoader.LoadXml("CustomXmlInitialization.xml");
            var riskNode = doc.SelectSingleNode("Tests/MainLocation");

            var factory = new CustomXmlCreation();

            CustomRisk risk = factory.Create<CustomRisk>(riskNode);
            Assert.IsNotNull(risk);
            Assert.That(risk.Location, Is.EqualTo("Hillside"));
            Assert.That(risk.Age, Is.EqualTo(34));
        }
    }

    class CustomRisk : ICustomXmlInitialization
    {
        private int m_Age;
        private string m_Location;

        public int Age
        {
            get{return m_Age;}
        }

        public string Location
        {
            get{return m_Location;}
        }

        void ICustomXmlInitialization.InitializeObject(XmlNode rootNode, ICustomXmlCreation factory)
        {
            m_Age = factory.Create<int>(rootNode.SelectSingleNode("Age|@Age"));
            m_Location = factory.Create<string>(rootNode.SelectSingleNode("Location|@Location"));
        }
    }
}
