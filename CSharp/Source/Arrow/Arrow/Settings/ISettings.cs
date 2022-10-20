using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Settings
{
	/// <summary>
	/// Defines access to name/value settings
	/// </summary>
	public interface ISettings
	{
		/// <summary>
		/// Returns the value for a named setting
		/// </summary>
		/// <param name="name">The name of the setting to get</param>
		/// <returns>The value for the setting, or null if the setting does not exist</returns>
		object? GetSetting(string name);
	}
}
