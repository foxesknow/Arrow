using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using Arrow.Storage;

namespace Arrow.Configuration
{
	/// <summary>
	/// Loads a config file using the StorageManager
	/// </summary>
	public class StorageConfigFile : IConfigFile
	{
		private readonly Uri m_Location;

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="location">The location of the config file</param>
		public StorageConfigFile(Uri location)
		{
			if(location==null) throw new ArgumentNullException("location");
			m_Location=location;
		}

		/// <summary>
		/// Loads the XmlDocument from the location
		/// </summary>
		/// <returns>An xml document</returns>
		public XmlDocument? LoadConfig()
		{
			return StorageManager.Get(m_Location).ReadXmlDocument();
		}

		/// <summary>
		/// The location of the config file
		/// </summary>
		public Uri? Uri
		{
			get{return m_Location;}
		}
	}
}
