using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Arrow.Xml
{
	/// <summary>
	/// Useful Xml extension methods
	/// </summary>
	public static class XmlExtensions
	{
		/// <summary>
		/// Looks up an attribute, returning the value it holds, or defaultValue if not found
		/// </summary>
		/// <param name="attributes">The attribute collection</param>
		/// <param name="name">The qualified name of the attribute</param>
		/// <param name="defaultValue">The value to return if the attribute does not exist</param>
		/// <returns>A value</returns>
		public static string GetValueOrDefault(this XmlAttributeCollection attributes, string name, string defaultValue)
		{
			XmlAttribute attr=attributes[name];
			return attr!=null ? attr.Value : defaultValue;
		}
		
		/// <summary>
		/// Looks up an attribute, returning the value it holds, or defaultValue if not found
		/// </summary>
		/// <param name="attributes">The attribute collection</param>
		/// <param name="localName">The local name of the attribute</param>
		/// <param name="namespaceURI">The namespace URI of the attribute</param>
		/// <param name="defaultValue">The value to return if the attribute does not exist</param>
		/// <returns>A value</returns>
		public static string GetValueOrDefault(this XmlAttributeCollection attributes, string localName, string namespaceURI, string defaultValue)
		{
			XmlAttribute attr=attributes[localName,namespaceURI];
			return attr!=null ? attr.Value : defaultValue;
		}

		/// <summary>
		/// Converts an XmlAttributeCollection to a typed sequence
		/// </summary>
		/// <param name="attributes">The attributes to convert</param>
		/// <returns>A typed sequence</returns>
		public static IEnumerable<XmlAttribute> AsEnumerable(this XmlAttributeCollection attributes)
		{
			return attributes.Cast<XmlAttribute>();
		}

		/// <summary>
		/// Converts an XmlNodeList to a typed sequence
		/// </summary>
		/// <param name="nodes">The nodes to convert</param>
		/// <returns>A typed sequence</returns>
		public static IEnumerable<XmlNode> AsEnumerable(this XmlNodeList nodes)
		{
			return nodes.Cast<XmlNode>();
		}
	}
}
