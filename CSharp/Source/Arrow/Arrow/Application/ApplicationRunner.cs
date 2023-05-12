using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Arrow.Logging;
using Arrow.Threading.Tasks;

namespace Arrow.Application
{
    /// <summary>
    /// Starts an application so that all Arrow susbystems are started and stopped in an orderly manner
    /// </summary>
    public static partial class ApplicationRunner
    {
        /// <summary>
        /// Runs a "main" method. Any failure to launch the application is logged.
        /// </summary>
        /// <param name="main">The method to run</param>
        /// <param name="args">The arguments to pass to the main delegate</param>
        public static Task Run(Func<string[], ValueTask> main, string[] args)
        {
            Func<string[], ValueTask<int>> wrapper = async (wrapperArgs) =>
            {
                await main(wrapperArgs).ContinueOnAnyContext();
                return 0;
            };

            return RunAndReturn(wrapper, args);
        }

        /// <summary>
        /// Runs a "main" method. Any failure to launch the application is logged.
        /// </summary>
        /// <param name="main">The action that is the main function of the application</param>
        public static Task Run(Func<Task> main)
        {
            ArgumentNullException.ThrowIfNull(main);

            return Execute(main);

            static async Task Execute(Func<Task> main)
            {
                // Start the systemwide services
                await using (ApplicationRunContext context = new ApplicationRunContext())
                {
                    await context.Start();

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
        }


        /// <summary>
        /// Runs a "main" method. Any failure to launch the application is logged.
        /// This overloads allows an exit code to be returned by the application
        /// </summary>
        /// <param name="main">The method to run</param>
        /// <param name="args">The arguments to pass to the main delegate</param>
        public static Task RunAndReturn(Func<string[], ValueTask<int>> main, string[] args)
        {
            ArgumentNullException.ThrowIfNull(main);
            ArgumentNullException.ThrowIfNull(args);

            return Execute(main, args);

            static async Task Execute(Func<string[], ValueTask<int>> main, string[] args)
            {
                // Start the systemwide services
                await using (ApplicationRunContext context = new ApplicationRunContext())
                {
                    await context.Start();

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
}
