using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Configuration;
using Arrow.Text;

namespace Arrow.Settings
{
    public sealed class AppSettingsProvider : ISettings
    {
        bool ISettings.TryGetSetting(string name, [NotNullWhen(true)] out object? value)
        {
            var setting = AppConfig.AppSettings[name];
            if(setting is not null)
            {
                setting = TokenExpander.ExpandText(setting);
            }

            value = setting;
            return value is not null;
        }
    }
}
