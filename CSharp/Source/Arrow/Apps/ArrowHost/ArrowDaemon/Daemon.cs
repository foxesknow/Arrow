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
        protected override void StartDaemon(string[] args)
        {
            Console.WriteLine("StartDaemon");
        }

        protected override void StopDaemon()
        {
            Console.WriteLine("StopDaemon");
        }
    }
}
