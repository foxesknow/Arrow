using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
		/// <param name="value">On success the value for the setting, null if the setting does not exist</param>
		/// <returns>true if the setting exists, otherwise false</returns>
		bool TryGetSetting(string name, [NotNullWhen(true)]out object? value);
	}
}
