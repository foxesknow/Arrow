using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Arrow.Xml.ObjectCreation
{
	/// <summary>
	/// Allows a class to take control of its own initialization via xml
	/// </summary>
	public interface ICustomXmlInitialization
	{
		/// <summary>
		/// Allows the object to initialize itself directly from xml
		/// </summary>
		/// <param name="rootNode">The root xml definition of the object</param>
		/// <param name="factory">The factory to use if creating child objects</param>
		void InitializeObject(XmlNode rootNode, CustomXmlCreation factory);
	}
}
