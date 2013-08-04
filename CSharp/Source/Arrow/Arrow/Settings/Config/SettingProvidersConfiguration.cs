using System;
using System.Collections.Generic;
using System.Text;

namespace Arrow.Settings.Config
{
	/// <summary>
	/// Holds information read from a <![CDATA[ <provider> ]]> element
	/// </summary>
	public class SettingProvidersConfiguration
	{
		private List<ProviderInfo> m_ProviderInfo=new List<ProviderInfo>();
		
		/// <summary>
		/// Adds a provider to the list
		/// </summary>
		/// <param name="info">The provider to add</param>
		internal void Add(ProviderInfo info)
		{
			m_ProviderInfo.Add(info);
		}
		
		/// <summary>
		/// Registers all setting proviers with the <b>GlobalSetting</b> class
		/// </summary>
		public void Apply()
		{
			foreach(ProviderInfo info in m_ProviderInfo)
			{
				SettingsManager.Register(info.Namespace,info.Settings);
			}
		}
	}
}
