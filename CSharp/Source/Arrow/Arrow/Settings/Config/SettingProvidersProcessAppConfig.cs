using System;
using System.Collections.Generic;
using System.Text;

using Arrow.Configuration;

namespace Arrow.Settings.Config
{
	/// <summary>
	/// Processes any Arrow.Settings section in the app.config file
	/// </summary>
	public static class SettingProvidersProcessAppConfig
	{
		private static bool s_DoneInit;
		private static readonly object s_SyncRoot=new object();
		
		/// <summary>
		/// It is safe to call this method multiple times as it will only process the section once
		/// </summary>
		public static void Process()
		{
			lock(s_SyncRoot)
			{
				if(s_DoneInit==false)
				{
					s_DoneInit=true;
					
					SettingProvidersConfiguration config=AppConfig.GetSectionForHandler<SettingProvidersSectionHandler>(ArrowSystem.Name,"Arrow.Settings") as SettingProvidersConfiguration;			
					if(config!=null)
					{
						config.Apply();
					}
				}
			}
		}
	}
}
