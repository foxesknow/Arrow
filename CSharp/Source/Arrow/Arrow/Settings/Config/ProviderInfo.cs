using System;
using System.Collections.Generic;
using System.Text;

namespace Arrow.Settings.Config
{
	/// <summary>
	/// Holds provider information thats been read from the app.config
	/// </summary>
	class ProviderInfo
	{
		private string m_Namespace;
		private ISettings m_Settings;
		
		/// <summary>
		/// The namespace to register the setting provider under
		/// </summary>
		public string Namespace
		{
			get{return m_Namespace;}
			set{m_Namespace=value;}
		}
		
		/// <summary>
		/// The settings provider to register
		/// </summary>
		public ISettings Settings
		{
			get{return m_Settings;}
			set{m_Settings=value;}
		}
	}
}
