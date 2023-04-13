using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Settings
{
    /// <summary>
    /// Treats the setting name as the value of the setting.
    /// 
    /// This is useful when using the first-of setting and you 
    /// want to specify a literal value in the last place as a default.
    /// </summary>
    public sealed class LiteralSettings : ISettings
    {
        public bool TryGetSetting(string name, [NotNullWhen(true)] out object? value)
        {
            value = name;
            return value is not null;
        }
    }
}
