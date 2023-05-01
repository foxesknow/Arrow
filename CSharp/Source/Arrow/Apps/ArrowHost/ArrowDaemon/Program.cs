using System.Threading.Tasks;

using Arrow.Application;
using Arrow.Application.DaemonHosting;

namespace ArrowDaemon
{
    internal class Program
    {
        /// <summary>
		/// The main entry point for the application.
		/// </summary>
		static Task Main(string[] args)
		{
			return ApplicationRunner.RunAsync(async  () => await Run(args));
		}

		static async ValueTask Run(string[] args)
		{
			//var runner = new ConsoleRunner<Daemon>();
			var runner = new HeadlessRunner<Daemon>();
			await runner.Run(args);
		}
    }
}