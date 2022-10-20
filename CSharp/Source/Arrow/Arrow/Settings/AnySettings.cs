using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Settings
{
	/// <summary>
	/// Looks through the settings stack for a particular setting
	/// </summary>
	public class AnySettings : ISettings
	{
		/// <summary>
		/// A shared instance
		/// </summary>
		public static readonly ISettings Instance=new AnySettings();

		/// <summary>
		/// Scans the settings stack held by the SettingsManager looking for any
		/// ISettings instance that can provide the specified setting
		/// </summary>
		/// <param name="name">The name of the setting to retrieve</param>
		/// <returns>The value of the setting, of null if it could not be found</returns>
		public object? GetSetting(string name)
		{
			object? value=null;
		
			List<string> stack=SettingsManager.NamespaceStack;
			
			for(int i=0; i<stack.Count && value==null; i++)
			{
				string @namespace=stack[i];
				var settings=SettingsManager.GetSettings(@namespace);
				
				if(settings==null) continue;
				
				value=settings.GetSetting(name);
			}
			
			
			return value;
		}
	}
}
