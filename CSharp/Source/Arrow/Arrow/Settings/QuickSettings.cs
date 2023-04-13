using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Settings
{
    /// <summary>
    /// Provides a quick way to create settings in an application and
    /// progmatically register them
    /// </summary>
    public sealed class QuickSettings : ISettings
    {
        private readonly IReadOnlyDictionary<string, object> m_Settings;

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="settings"></param>
        public QuickSettings(params (string Name, object value)[] settings)
            :this((IEnumerable<(string Name, object value)>)settings)
        {
        }

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="settings"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public QuickSettings(IEnumerable<(string Name, object value)> settings)
        {
            if(settings is null) throw new ArgumentNullException(nameof(settings));

            m_Settings = settings.ToDictionary(pair => pair.Name, pair => pair.value, StringComparer.OrdinalIgnoreCase);
        }

        /// <inheritdoc/>
        public bool TryGetSetting(string name, [NotNullWhen(true)] out object? value)
        {
            return m_Settings.TryGetValue(name, out value);
        }
    }
}
