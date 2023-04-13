using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Settings
{
    /// <summary>
    /// A setting provider that returns pre-defined string.
    /// </summary>
    public sealed class StringSettings : ISettings
    {
        /// <inheritdoc/>
        public bool TryGetSetting(string name, [NotNullWhen(true)] out object? value)
        {
            value = name.ToLower() switch
            {
                "empty" => "",
                _       => null
            };

            return value is not null;
        }
    }
}
