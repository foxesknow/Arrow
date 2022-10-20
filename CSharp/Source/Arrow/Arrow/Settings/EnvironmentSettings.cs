using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Settings
{
	/// <summary>
	/// Provides access to settings stored in environmental variables
	/// </summary>
	public class EnvironmentSettings : ISettings
	{
		/// <summary>
		/// An instance that may be shared
		/// </summary>
		public static readonly ISettings Instance=new EnvironmentSettings();

		/// <summary>
		/// Retrieves a setting from the environment
		/// </summary>
		/// <param name="name">The environment variable to fetch</param>
		/// <returns>The value of the variable</returns>
		public object? GetSetting(string name)
		{
			return Environment.GetEnvironmentVariable(name);
		}
	}
}
