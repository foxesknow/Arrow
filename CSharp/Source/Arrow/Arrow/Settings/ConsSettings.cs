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
    public sealed class ConsSetting : ISettings
    {
        private readonly ISettings m_Head;
        private readonly ISettings m_Tail;

        /// <summary>
        /// Initializes the instance
        /// </summary>
        public ConsSetting(ISettings head, ISettings tail)
        {
            if(head is null) throw new ArgumentNullException(nameof(head));
            if(tail is null) throw new ArgumentNullException(nameof(tail));

            m_Head = head;
            m_Tail = tail;
        }


        /// <summary>
        /// Checks the head for the setting, otherwise looks in the tail
        /// </summary>
        /// <param name="name">The name of the setting</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetSetting(string name, [NotNullWhen(true)] out object? value)
        {
            if(m_Head.TryGetSetting(name, out value))
            {
                return true;
            }

            return m_Tail.TryGetSetting(name, out value);
        }
    }
}
