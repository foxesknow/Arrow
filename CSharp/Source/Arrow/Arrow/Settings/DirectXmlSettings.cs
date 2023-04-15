using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Xml;

using Arrow.Collections;
using Arrow.Xml;
using Arrow.Xml.ObjectCreation;

namespace Arrow.Settings
{
    /// <summary>
    /// Loads settings directly from xml, usually in app.config
    /// For example:
    /// <![CDATA[
    /// <Arrow>
    ///		<Arrow.Settings>
    ///			<Setting>
    ///				<Namespace>Person</Namespace>
    ///				<Provider type="Arrow.Settings.DirectXmlSettings, Arrow">
    ///					<Name>Sawyer</Name>
    ///					<Location>Island</Location>
    ///				</Provider>
    ///			</Setting>
    ///		</Arrow.Settings>
    /// </Arrow>
    /// 
    /// ]]>
    /// </summary>
    public sealed class DirectXmlSettings : ISettings, ICustomXmlInitialization
    {
        private readonly Dictionary<string, object> m_Settings = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public bool TryGetSetting(string name, [NotNullWhen(true)] out object? value)
        {
            return m_Settings.TryGetValue(name, out value);
        }

        void ICustomXmlInitialization.InitializeObject(XmlNode rootNode, ICustomXmlCreation factory)
        {
            XmlNodeList nodes = rootNode.SelectNodesOrEmpty("*");
            factory.PopulateDictionary(m_Settings, nodes);
        }
    }
}
