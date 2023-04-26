using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using Arrow.Logging;
using Arrow.Threading;
using Arrow.Threading.Tasks;

namespace Arrow.Application
{
    public static partial class ApplicationRunner
    {
        /// <summary>
        /// Runs a "main" method. Any failure to launch the application is logged.
        /// </summary>
        /// <param name="main">The method to run</param>
        /// <param name="args">The arguments to pass to the main delegate</param>
        public static Task RunAsync(Func<string[], Task> main, string[] args)
        {
            Func<string[], Task<int>> wrapper = async (wrapperArgs) =>
            {
                await main(wrapperArgs).ContinueOnAnyContext();
                return 0;
            };

            return RunAndReturnAsync(wrapper, args);
        }

        /// <summary>
        /// Runs a "main" method. Any failure to launch the application is logged.
        /// </summary>
        /// <param name="main">The action that is the main function of the application</param>
        public static async Task RunAsync(Func<Task> main)
        {
            if (main is null) throw new ArgumentNullException("main");

            // Start the systemwide services
            using (ApplicationRunContext context = new ApplicationRunContext())
            {
                try
                {
                    await main().ContinueOnAnyContext();
                }
                catch (Exception e)
                {
                    ILog log = LogManager.GetLog(typeof(ApplicationRunner));
                    log.Error("Error running application", e);
                    Environment.ExitCode = -1;
                }
            }
        }


        /// <summary>
        /// Runs a "main" method. Any failure to launch the application is logged.
        /// This overloads allows an exit code to be returned by the application
        /// </summary>
        /// <param name="main">The method to run</param>
        /// <param name="args">The arguments to pass to the main delegate</param>
        public static async Task RunAndReturnAsync(Func<string[], Task<int>> main, string[] args)
        {
            if (main is null) throw new ArgumentNullException("main");
            if (args is null) throw new ArgumentNullException("args");

            // Start the systemwide services
            using (ApplicationRunContext context = new ApplicationRunContext())
            {
                try
                {
                    int exitCode = await main(args).ContinueOnAnyContext();
                    Environment.ExitCode = exitCode;
                }
                catch (Exception e)
                {
                    ILog log = LogManager.GetLog(typeof(ApplicationRunner));
                    log.Error("Error running application", e);
                    Environment.ExitCode = -1;
                }
            }

        }
    }
}
