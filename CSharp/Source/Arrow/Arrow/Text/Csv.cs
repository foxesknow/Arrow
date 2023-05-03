using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Text
{
    public static class Csv
    {
        public static string Escape(string value)
        {
            if(value is null) throw new ArgumentNullException(nameof(value));

            bool wrapInQuotes = false;
            StringBuilder? b = null;

            for(int i = 0; i < value.Length; i++)
            {
                char c = value[i];

                if(c == '\"')
                {
                    if(b is null)
                    {
                        b = new StringBuilder(value.Length + 10);
                        b.Append(value.AsSpan(0, i));
                    }

                    b.Append(c);
                }
                else if(c == ',')
                {
                    wrapInQuotes = true;
                }

                if(b is not null) b.Append(c);
            }

            var finalValue = (b is null ? value : b.ToString());

            if(wrapInQuotes)
            {
                return string.Concat("\"", finalValue, "\"");
            }
            
            return finalValue;

        }
    }
}
