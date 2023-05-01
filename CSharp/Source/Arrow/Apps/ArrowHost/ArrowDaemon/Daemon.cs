using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Application.DaemonHosting;

namespace ArrowDaemon
{
    internal class Daemon : DaemonBase
    {
        protected override ValueTask StartDaemon(string[] args)
        {
            Console.WriteLine("StartDaemon");
            return default;
        }

        protected override ValueTask StopDaemon()
        {
            Console.WriteLine("StopDaemon");
            return default;
        }
    }
}
