using System;
using System.Collections.Generic;
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
	public class DirectXmlSettings : ISettings, ICustomXmlInitialization
	{
		private Dictionary<string,object> m_Settings=new Dictionary<string,object>(IgnoreCaseEqualityComparer.Instance);
	
		object? ISettings.GetSetting(string name)
		{
			m_Settings.TryGetValue(name,out var value);
			
			return value;
		}

		void ICustomXmlInitialization.InitializeObject(XmlNode rootNode, CustomXmlCreation factory)
		{
			XmlNodeList nodes=rootNode.SelectNodesOrEmpty("*");
			factory.PopulateDictionary(m_Settings,nodes);
		}
	}
}
