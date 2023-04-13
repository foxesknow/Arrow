using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

using Arrow.Calendar;

namespace Arrow.Settings
{
    /// <summary>
    /// Retrieves either Now or UtcNow
    /// </summary>
    public sealed class DateTimeSettings : ISettings
    {
        /// <summary>
        /// An instance that may be shared
        /// </summary>		
        public static readonly ISettings Instance = new DateTimeSettings();

        /// <inheritdoc/>
        public bool TryGetSetting(string name, [NotNullWhen(true)] out object? value)
        {
            switch(name.ToLower())
            {
                case "now":
                    value = Clock.Now;
                    return true;

                case "utcnow":
                    value = Clock.UtcNow;
                    return true;

                default:
                    value = null;
                    return false;
            }
        }
    }
}
