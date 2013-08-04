using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Arrow.Configuration
{
	/// <summary>
	/// Specifies how to source a config file for use within the configuration system
	/// </summary>
	public interface IConfigFile
	{
		/// <summary>
		/// Loads the config document.
		/// This method should never throw an exception. If the file cannot be
		/// loaded then it should return null
		/// </summary>
		/// <returns>The config document, or null if it could not be loaded</returns>
		XmlDocument LoadConfig();
		
		/// <summary>
		/// The location of the document
		/// </summary>
		Uri Uri{get;}
	}
}
