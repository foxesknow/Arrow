using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace Arrow.Configuration
{
	/// <summary>
	/// Grabs the application config file
	/// </summary>
	public class AppDomainConfigFile : IConfigFile
	{
		private string m_Filename;
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		public AppDomainConfigFile()
		{
			AppDomain appDomain=AppDomain.CurrentDomain;
			if(appDomain!=null)
			{
				AppDomainSetup setup=appDomain.SetupInformation;
				if(setup!=null)
				{
					m_Filename=setup.ConfigurationFile;
				}
			}
		}
	
		#region IConfigFile Members

		/// <summary>
		/// Loads the config file
		/// </summary>
		/// <returns>The config file, or null if the file could not be sourced</returns>
		public XmlDocument LoadConfig()
		{
			
			
			try
			{
				XmlDocument doc=null;
				if(m_Filename!=null && File.Exists(m_Filename))
				{
					doc=new XmlDocument();
					doc.Load(m_Filename);
				}
				
				return doc;
			}
			catch
			{
				return null;
			}
		}
		
		/// <summary>
		/// Returns a uri to the file location of the config
		/// </summary>
		public Uri Uri
		{
			get{return m_Filename==null ? null : new Uri(m_Filename);}
		}

		#endregion
	}
}
