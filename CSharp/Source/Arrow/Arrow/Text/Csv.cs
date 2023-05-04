using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Text
{
    /// <summary>
    /// Useful csv methods.
    /// </summary>
    public static class Csv
    {
        /// <summary>
        /// Escapes a value, if required
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string Escape(string value)
        {
            if(value is null) throw new ArgumentNullException(nameof(value));

            bool wrapInQuotes = false;
            StringBuilder? b = null;

            for(var i = 0; i < value.Length; i++)
            {
                char c = value[i];

                if(c == '\"')
                {
                    if(b is null)
                    {
                        // *2 as worst case evey character will be escaped
                        // +2 as we may have to put quotes at the beginning and end
                        b = new StringBuilder((value.Length * 2) + 2);
                        b.Append(value.AsSpan(0, i));
                    }

                    b.Append(c);
                }
                else if(wrapInQuotes == false && (c == ',' || c == '\r' || c == '\n'))
                {
                    wrapInQuotes = true;
                }

                if(b is not null) b.Append(c);
            }

            // If we've got a builder then we can use it to 
            // more efficiently wrap the string
            if(b is not null)
            {
                if(wrapInQuotes)
                {
                    b.Insert(0, '\"');
                    b.Append('\"');
                }

                return b.ToString();
            }

            if(wrapInQuotes)
            {
                return string.Concat("\"", value, "\"");
            }
            
            return value;

        }
    }
}
