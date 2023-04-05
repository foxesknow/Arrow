using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        public static readonly ISettings Instance = new EnvironmentSettings();

        /// <inheritdoc/>
        public bool TryGetSetting(string name, [NotNullWhen(true)] out object? value)
        {
            value = Environment.GetEnvironmentVariable(name);
            return value is not null;
        }
    }
}
