using Arrow.Application;
using Arrow.Application.DaemonHosting;

namespace ArrowDaemon
{
    internal class Program
    {
        /// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args)
		{
			ApplicationRunner.Run(()=>Run(args));
		}

		static void Run(string[] args)
		{
			//var runner = new ConsoleRunner<Daemon>();
			var runner = new HeadlessRunner<Daemon>();
			runner.Run(args);
		}
    }
}