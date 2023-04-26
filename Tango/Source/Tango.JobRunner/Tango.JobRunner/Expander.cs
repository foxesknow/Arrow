using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Text;

namespace Tango.JobRunner
{
    public sealed class Expander
    {
        private const string BeginToken = "$(";
        private const string EndToken = ")";

        private readonly Dictionary<string, object> m_Mappings = new(StringComparer.OrdinalIgnoreCase);

        public void Add(string key, object value)
        {
            m_Mappings[key] = value;
        }

        public void AddDates(DateTime when)
        {
            Add("YYYYMMDD", when.ToString("yyyyMMdd"));
            Add("DDMMMYYYY", when.ToString("ddMMMyyyy"));
            Add("date", when);
        }

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
