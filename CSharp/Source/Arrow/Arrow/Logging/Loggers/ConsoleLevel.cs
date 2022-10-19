using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Logging.Loggers
{
    readonly struct ConsoleLevel
    {
        public ConsoleLevel(string prefix, ConsoleColor color)
        {
            this.Prefix = prefix;
            this.Color = color;
        }

        public ConsoleColor Color{get;}

        public string Prefix{get;}

        public override string ToString()
        {
            return this.Prefix;
        }
    }
}
