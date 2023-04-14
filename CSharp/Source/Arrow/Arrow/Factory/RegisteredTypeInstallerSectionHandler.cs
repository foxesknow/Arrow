using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Xml;

namespace Arrow.Factory
{
	/// <summary>
	/// Parses a node that contains a series of <![CDATA[ <Assembly> ]]> elements
	/// </summary>
	public class RegisteredTypeInstallerSectionHandler : IConfigurationSectionHandler
	{
		/// <summary>
		/// Creates a <b>RegisteredTypeInstallerConfiguration</b> instance
		/// </summary>
		/// <param name="parent">The parent</param>
		/// <param name="configContext">The config context</param>
		/// <param name="section">A node containing a series of <b>Assembly</b> elements</param>
		/// <returns>A RegisteredTypeInstallerConfiguration instance</returns>
		public object Create(object parent, object configContext, XmlNode section)
		{
			return RegisteredTypeInstallerConfiguration.FromXml(section);
		}
	}
}
