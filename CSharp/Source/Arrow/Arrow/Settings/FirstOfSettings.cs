using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Text;

namespace Arrow.Settings
{
    /// <summary>
    /// Allows a sequence of settings to be provided, separated by ??.
    /// Each setting is evaluated, and the first to return a non-null value is returned.
    
    /// For example:
    /// 
    /// first-of: appSettings:temp ?? env:temp
    /// </summary>
    public sealed class FirstOfSettings : ISettings
    {
        private static readonly string[] Separator = new[]{"??"};

        /// <inheritdoc/>
        public bool TryGetSetting(string name, [NotNullWhen(true)] out object? value)
        {
            var allSettings = ExtractSettings(name);

            foreach(var setting in allSettings)
            {
                SettingsManager.CrackQualifiedName(setting, out var @namespace, out var _);
                if(SettingsManager.IsRegistered(@namespace))
                {
                    value = TokenExpander.ExpandToken(setting);
                    if(value is not null) return true;
                }
            }

            value = null;
            return false;
        }

        private static string[] ExtractSettings(string allSettings)
        {
            return allSettings.Split(Separator, StringSplitOptions.RemoveEmptyEntries)
                              .Select(part => part.Trim())
                              .ToArray();
        }
    }
}
