using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Arrow.Settings
{
    /// <summary>
    /// Allows another setting lookup to occur, and if that setting does
    /// not exist then a default value is returned
    /// </summary>
    public sealed class DefaultSettings : ISettings
    {
        /// <summary>
        /// A shared instance
        /// </summary>
        public static readonly ISettings Instance = new DefaultSettings();

        /// <summary>
        /// Returns a default value if the setting could be be retrieved.
        /// The setting must have the format defaultValue:namespace:setting
        /// </summary>
        /// <param name="name">The name of the setting to get</param>
        /// <param name="value"></param>
        /// <returns>Always returns true</returns>
        public bool TryGetSetting(string name, [NotNullWhen(true)] out object? value)
        {
            if(name is null) throw new ArgumentNullException("name");

            string? defaultValue = null;
            string? setting = null;

            char separator = SettingsManager.NamespaceSeparatorChar;

            if(name.StartsWith("\""))
            {
                // It's a quoted string
                int pivot = name.IndexOf('\"', 1);
                if(pivot == -1) throw new ArrowException("DefaultSettings - could not find end qute");
                if(name[pivot + 1] != separator) throw new ArrowException("DefaultSettings - quoted strings must be followed by a namespace separator");

                defaultValue = name.Substring(1, pivot - 1);
                setting = name.Substring(pivot + 2); // +2 to skip the quote and the namespace seperator
            }
            else
            {
                int pivot = name.IndexOf(separator);
                if(pivot == -1) throw new ArrowException("DefaultSettings - could not find pivot");

                defaultValue = name.Substring(0, pivot);
                setting = name.Substring(pivot + 1);
            }

            if(SettingsManager.TryGetSetting<object>(setting, out value))
            {
                return true;
            }

            value = defaultValue;
            return true;
        }
    }
}
