using Arrow.Application;
using Arrow.Application.Daemon;

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
			var daemon = new ConsoleDaemon<ApplicationDaemon>();
			daemon.Run(args);
		}
    }
}