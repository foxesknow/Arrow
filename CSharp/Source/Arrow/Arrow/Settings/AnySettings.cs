using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Arrow.Settings
{
    /// <summary>
    /// Looks through the settings stack for a particular setting
    /// </summary>
    public sealed class AnySettings : ISettings
    {
        /// <summary>
        /// A shared instance
        /// </summary>
        public static readonly ISettings Instance = new AnySettings();

        /// <summary>
        /// Scans the settings stack held by the SettingsManager looking for any
        /// ISettings instance that can provide the specified setting
        /// </summary>
        /// <param name="name">The name of the setting to retrieve</param>
        /// <param name="value"
        /// <returns></returns>
        public bool TryGetSetting(string name, [NotNullWhen(true)] out object? value)
        {
            value = null;

            List<string> stack = SettingsManager.NamespaceStack;

            for(int i = 0; i < stack.Count && value == null; i++)
            {
                string @namespace = stack[i];
                var settings = SettingsManager.GetSettings(@namespace);

                if(settings == null) continue;

                if(settings.TryGetSetting(name, out value)) return true;
            }

            return false;
        }
    }
}
