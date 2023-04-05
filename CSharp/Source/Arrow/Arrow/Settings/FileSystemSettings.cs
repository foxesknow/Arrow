using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics.CodeAnalysis;

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
	
		/// <inheritdoc/>
		public bool TryGetSetting(string name, [NotNullWhen(true)] out object? value)
		{
			switch(name.ToLower())
			{
				case "cwd":
					value = Directory.GetCurrentDirectory();
					return true;
					
				case "tempdir":
					value = Path.GetTempPath();
					return true;
					
				case "randomname":
					value = Path.GetRandomFileName();
					return true;
			
				default:
					value = null;
					return false;
			}
		}
	}
}
