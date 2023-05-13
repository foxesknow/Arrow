using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTests.Arrow.Support
{
    [Flags]
    public enum Mode
    {
        None = 0,
        Read = 1,
        Write = 2,
        Execute = 4
    }
}
