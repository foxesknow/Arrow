using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Arrow.Text.Json
{
    public abstract class JsonTestBase
    {
        /// <summary>
        /// Replaces ' with " in a Json string.
        /// This makes is easier to write Json in source files without escaping the quotes.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected string ToJson(string value)
        {
            return value.Replace('\'', '"');
        }
    }
}
