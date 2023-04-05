using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Logging;

namespace Arrow.Application
{
    /// <summary>
    /// Starts an application so that all Arrow susbystems are started and stopped in an orderly manner
    /// </summary>
    public static class ApplicationRunner
    {
        /// <summary>
        /// Runs a "main" method. Any failure to launch the application is logged.
        /// </summary>
        /// <param name="main">The method to run</param>
        /// <param name="args">The arguments to pass to the main delegate</param>
        public static void Run(Action<string[]> main, string[] args)
        {
            Func<string[], int> wrapper = (string[] wrapperArgs) =>
            {
                main(wrapperArgs);
                return 0;
            };

            RunAndReturn(wrapper, args);
        }

        /// <summary>
        /// Runs a "main" method. Any failure to launch the application is logged.
        /// </summary>
        /// <param name="main">The action that is the main function of the application</param>
        public static void Run(Action main)
        {
            if(main == null) throw new ArgumentNullException("main");

            // Start the systemwide services
            using(ApplicationRunContext context = new ApplicationRunContext())
            {
                try
                {
                    main();
                }
                catch(Exception e)
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
        public static void RunAndReturn(Func<string[], int> main, string[] args)
        {
            if(main == null) throw new ArgumentNullException("main");
            if(args == null) throw new ArgumentNullException("args");

            // Start the systemwide services
            using(ApplicationRunContext context = new ApplicationRunContext())
            {
                try
                {
                    int exitCode = main(args);
                    Environment.ExitCode = exitCode;
                }
                catch(Exception e)
                {
                    ILog log = LogManager.GetLog(typeof(ApplicationRunner));
                    log.Error("Error running application", e);
                    Environment.ExitCode = -1;
                }
            }

        }
    }
}
