using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Configuration;

namespace Arrow.Configuration
{
	/// <summary>
	/// Returns the xml contained with a section
	/// </summary>
	public class ArrowSectionHandler : IConfigurationSectionHandler
	{
		#region IConfigurationSectionHandler Members

		/// <summary>
		/// Returns the xml within a section
		/// </summary>
		/// <param name="parent">The parent object</param>
		/// <param name="configContext">The configuration context</param>
		/// <param name="section">The section xml</param>
		/// <returns>The section xml</returns>
		public object Create(object parent, object configContext, XmlNode section)
		{
			return section;
		}

		#endregion
	}
}
