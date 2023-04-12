using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using Arrow.Configuration;

namespace UnitTests.Arrow.Configuration
{
    internal class XmlTextConfigFile : IConfigFile
    {
        private readonly XmlDocument m_Document;

        public XmlTextConfigFile(string xml)
        {
            m_Document = new XmlDocument();
            m_Document.LoadXml(xml);
        }

        Uri IConfigFile.Uri => null;

        XmlDocument IConfigFile.LoadConfig()
        {
            return m_Document;
        }
    }
}
