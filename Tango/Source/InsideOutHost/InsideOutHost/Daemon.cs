using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Application.DaemonHosting;

namespace InsideOutHost
{
    internal class Daemon : DaemonBase
    {
        protected override ValueTask StartDaemon(string[] args)
        {
            return default;
        }

        protected override ValueTask StopDaemon()
        {
            return default;
        }
    }
}
