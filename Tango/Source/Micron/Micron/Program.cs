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
			return ApplicationRunner.RunAsync(async () => await Run(args));
		}

		static async ValueTask Run(string[] args)
		{
			var runner = new HeadlessRunner<Daemon>();
			await runner.Run(args);
		}
    }
}