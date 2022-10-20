using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Settings
{
	/// <summary>
	/// Allows another setting lookup to occur, and if that setting does
	/// not exist then a default value is returned
	/// </summary>
	public class DefaultSettings : ISettings
	{
		/// <summary>
		/// A shared instance
		/// </summary>
		public static readonly ISettings Instance=new DefaultSettings();

		/// <summary>
		/// Returns a default value if the setting could be be retrieved.
		/// The setting must have the format defaultValue:namespace:setting
		/// </summary>
		/// <param name="name">The name of the setting to get</param>
		/// <returns>The value of the setting, if found, otherwise the default value</returns>
		public object? GetSetting(string name)
		{
			if(name==null) throw new ArgumentNullException("name");
			
			string? defaultValue=null;
			string? setting=null;
			
			char separator=SettingsManager.NamespaceSeparatorChar;
			
			if(name.StartsWith("\""))
			{
				// It's a quoted string
				int pivot=name.IndexOf('\"',1);
				if(pivot==-1) throw new ArrowException("DefaultSettings - could not find end qute");
				if(name[pivot+1]!=separator) throw new ArrowException("DefaultSettings - quoted strings must be followed by a namespace separator");
				
				defaultValue=name.Substring(1,pivot-1);
				setting=name.Substring(pivot+2); // +2 to skip the quote and the namespace seperator
			}
			else
			{
				int pivot=name.IndexOf(separator);
				if(pivot==-1) throw new ArrowException("DefaultSettings - could not find pivot");
				
				defaultValue=name.Substring(0,pivot);
				setting=name.Substring(pivot+1);
			}
			
			SettingsManager.TryGetSetting<object>(setting,out var value);
			
			return value ?? defaultValue;
		}
	}
}
