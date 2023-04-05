using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Arrow.Settings
{
    /// <summary>
    /// Groups multiple settings into one
    /// </summary>
    public class SettingsCollection : ISettings
    {
        private readonly List<ISettings> m_Settings = new List<ISettings>();

        /// <summary>
        /// Initializes the instance
        /// </summary>
        public SettingsCollection()
        {
        }

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="settings">The settings to initialize the collection with</param>
        public SettingsCollection(IEnumerable<ISettings> settings)
        {
            if(settings == null) throw new ArgumentNullException("settings");
            m_Settings.AddRange(settings);
        }

        /// <summary>
        /// Adds a new setting
        /// </summary>
        /// <param name="settings">The settings to add</param>
        public void Add(ISettings settings)
        {
            if(settings == null) throw new ArgumentNullException("settings");
            m_Settings.Add(settings);
        }

        /// <summary>
        /// Searches for a setting amongst the collection, searching from most recently added to least recently
        /// </summary>
        /// <param name="name">The name of the setting</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetSetting(string name, [NotNullWhen(true)] out object? value)
        {
            value = null;

            // Search in reverse to give the most recent priority
            for(int i = m_Settings.Count - 1; i >= 0 && value == null; i--)
            {
                ISettings settings = m_Settings[i];
                if(settings.TryGetSetting(name, out value))
                {
                    return true;
                }
            }

            value = null;
            return false;
        }
    }
}
