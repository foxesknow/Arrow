using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Arrow.Configuration
{
	/// <summary>
	/// Always return no configuration
	/// </summary>
	public class NullConfigFile : IConfigFile
	{
		/// <summary>
		/// Returns null
		/// </summary>
		/// <returns>Returns null</returns>
		public XmlDocument LoadConfig()
		{
			return null;
		}
		
		/// <summary>
		/// Returns null
		/// </summary>
		public Uri Uri
		{
			get{return null;}
		}
	}
}
