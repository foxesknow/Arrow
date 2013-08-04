using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Arrow.Settings
{
	/// <summary>
	/// Provides settings about the file system
	/// </summary>
	public class FileSystemSettings : ISettings
	{
		/// <summary>
		/// An instance of the class that may be shared
		/// </summary>
		public static readonly ISettings Instance=new FileSystemSettings();
	
		/// <summary>
		/// Retrieves a filesystem setting.
		/// </summary>
		/// <param name="name">The filesystem setting name</param>
		/// <returns>A string instance, or null if the setting does not exist</returns>
		public object GetSetting(string name)
		{
			switch(name.ToLower())
			{
				case "cwd":
					return Directory.GetCurrentDirectory();
					
				case "tempdir":
					return Path.GetTempPath();
					
				case "randomname":
					return Path.GetRandomFileName();
			
				default:
					return null;
			}
		}
	}
}
