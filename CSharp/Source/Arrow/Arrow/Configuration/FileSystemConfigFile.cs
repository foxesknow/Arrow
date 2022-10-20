using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace Arrow.Configuration
{
	/// <summary>
	/// Loads a config file directly from the filesystem
	/// </summary>
	public class FileSystemConfigFile : IConfigFile
	{
		private string m_Filename;
	
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="filename">The name of the file to load</param>
		/// <exception cref="System.ArgumentNullException">filename is null</exception>
		public FileSystemConfigFile(string filename)
		{
			if(filename==null) throw new ArgumentNullException(filename);
			
			m_Filename=filename;
		}

		/// <summary>
		/// Loads the config file
		/// </summary>
		/// <returns>The config file, or null if the file could not be sourced</returns>
		public XmlDocument? LoadConfig()
		{
			XmlDocument? doc=null;
			
			try
			{
				if(File.Exists(m_Filename))
				{
					doc=new XmlDocument();
					doc.Load(m_Filename);
				}
			}
			catch
			{
				doc=null;
			}
			
			return doc;
		}
		
		/// <summary>
		/// Returns the location of the file
		/// </summary>
		public Uri? Uri
		{
			get{return new Uri(m_Filename);}
		}

	}
}
