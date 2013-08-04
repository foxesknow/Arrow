using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Xml;

using Arrow.Xml.ObjectCreation;
using Arrow.Storage;
using Arrow.Text;

namespace Arrow.Settings.Config
{
	/// <summary>
	/// Parses a node that contains a series of <![CDATA[ <Setting> ]]> elements
	/// </summary>
	public class SettingProvidersSectionHandler : IConfigurationSectionHandler
	{
		#region IConfigurationSectionHandler Members

		/// <summary>
		/// Creates a <b>SettingProvidersConfiguration</b> instance
		/// </summary>
		/// <param name="parent">The parent</param>
		/// <param name="configContext">The config context</param>
		/// <param name="section">A node containing a series of <b>provider</b> elements</param>
		/// <returns>A SettingProvidersConfiguration instance</returns>
		public object Create(object parent, object configContext, XmlNode section)
		{
			SettingProvidersConfiguration config=new SettingProvidersConfiguration();
			ProcessSettings(config,section);
			ProcessSettingScript(config,section);			
			
			return config;
		}

		#endregion
		
		/// <summary>
		/// Scans the xml looking for "SettingScript" elements that contain like to external settings
		/// </summary>
		/// <param name="config">The config block to hold the providers</param>
		/// <param name="section">The xml to scan for "SettingScript" elements</param>
		private void ProcessSettingScript(SettingProvidersConfiguration config, XmlNode section)
		{
			foreach(XmlNode settingScriptNode in section.SelectNodes("SettingScript"))
			{
				XmlAttribute uriAttr=settingScriptNode.Attributes["uri"];
				if(uriAttr==null) continue;
				
				string uriText=TokenExpander.ExpandText(uriAttr.Value);
				Uri uri=Accessor.CreateUri(uriText);
				XmlDocument doc=StorageManager.Get(uri).ReadXmlDocument();
				ProcessSettings(config,doc.DocumentElement);
			}
		}
		
		/// <summary>
		/// Loads Settings elements from xml
		/// </summary>
		/// <param name="config">The config block to hold the providers</param>
		/// <param name="section">The xml to scan for "Setting" elements</param>
		private void ProcessSettings(SettingProvidersConfiguration config, XmlNode section)
		{
			// Each provider elements needs a "namespace" and "provider" value
			foreach(XmlNode settingNode in section.SelectNodes("Setting"))
			{
				ProviderInfo info=XmlCreation.Create<ProviderInfo>(typeof(ProviderInfo),settingNode);
				config.Add(info);
			}
		}
	}
}
