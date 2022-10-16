using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Threading.Tasks
{
    public static class DataTimeout
    {
        public static readonly TimeSpan WaitForever = TimeSpan.FromMilliseconds(-1);

        public static bool IsValid(TimeSpan timeSpan)
        {
            var ms = (long)timeSpan.Milliseconds;
            return ms >= -1;
        }
    }
}
