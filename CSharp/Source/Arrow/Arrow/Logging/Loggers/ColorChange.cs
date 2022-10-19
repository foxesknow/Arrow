using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Logging.Loggers
{
    readonly ref struct ColorChange
    {
        private readonly bool m_Redirected;

        public  ColorChange(ConsoleColor foreground, bool redirected)
        {
            m_Redirected = redirected;

            if(!redirected) Console.ForegroundColor = foreground;
        }

        public void Dispose()
        {
            if(m_Redirected) Console.ResetColor();
        }
    }
}
