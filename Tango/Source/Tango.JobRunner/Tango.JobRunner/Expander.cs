using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Text;

namespace Tango.JobRunner
{
    /// <summary>
    /// Expands job runner specific tokens.
    /// </summary>
    public sealed class Expander
    {
        private const string BeginToken = "$(";
        private const string EndToken = ")";

        private readonly Dictionary<string, object?> m_Mappings = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Adds a new token to the expander
        /// </summary>
        /// <param name="token"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Expander Add(string token, object? value)
        {
            m_Mappings[token] = value;
            return this;
        }

        /// <summary>
        /// Adds date tokens in several useful formats
        /// </summary>
        /// <param name="when"></param>
        /// <returns></returns>
        public Expander AddDates(DateTime when)
        {
            Add("YYYYMMDD", when.ToString("yyyyMMdd"));
            Add("DDMMMYYYY", when.ToString("ddMMMyyyy"));
            Add("date", when);

            return this;
        }

        /// <summary>
        /// Expands the text.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string Expand(string text)
        {
            return TokenExpander.ExpandText(text, BeginToken, EndToken, key =>
            {
                m_Mappings.TryGetValue(key, out var value);
                return value;
            });
        }
    }
}
