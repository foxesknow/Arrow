using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Logging.Loggers
{
    readonly ref struct ColorChange
    {
        public ColorChange(ConsoleColor foreground, bool colorize)
        {
            Console.ForegroundColor = foreground;
        }

        public void Dispose()
        {
            Console.ResetColor();
        }
    }
}
