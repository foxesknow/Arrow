using System;
using System.Threading.Tasks;

using Arrow.Application.DaemonHosting;
using Arrow.Application;

namespace Micron
{
    internal class Program
    {
        static Task Main(string[] args)
		{
			return ApplicationRunner.Run(() => Run(args));
		}

		static async Task Run(string[] args)
		{
			var runner = new HeadlessRunner<Daemon>();
			await runner.Run(args);
		}
    }
}