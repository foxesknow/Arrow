using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace UnitTests.Arrow.Xml.ObjectCreation
{
    public abstract class TestBase
    {
        protected XmlDocument XmlFromString(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            return doc;
        }
    }
}
