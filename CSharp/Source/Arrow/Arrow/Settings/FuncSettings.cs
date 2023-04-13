using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Settings
{
    /// <summary>
    /// Looks up a setting by calling a function
    /// </summary>
    public sealed class FuncSettings : ISettings
    {
        private readonly Func<string, object?> m_Lookup;

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="lookup"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public FuncSettings(Func<string, object?> lookup)
        {
            if(lookup is null) throw new ArgumentNullException(nameof(lookup));

            m_Lookup = lookup;
        }
        
        /// <inheritdoc/>
        public bool TryGetSetting(string name, [NotNullWhen(true)] out object? value)
        {
            value = m_Lookup(name);
            return value is not null;
        }
    }
}
