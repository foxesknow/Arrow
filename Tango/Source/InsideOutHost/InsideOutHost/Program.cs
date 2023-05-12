using System;
using System.Threading.Tasks;

using Arrow.Application;
using Arrow.Application.DaemonHosting;

namespace InsideOutHost
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await ApplicationRunner.Run(() => Run(args));
        }

        private static async Task Run(string[] args)
        {
            var daemon = new ConsoleRunner<Daemon>();
            await daemon.Run(args);
        }
    }
}