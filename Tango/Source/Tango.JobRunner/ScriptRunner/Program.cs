using System;
using System.Threading.Tasks;
using System.Xml;

using Arrow.Configuration;
using Arrow.Application;
using Arrow.Text;
using Arrow.Storage;
using Arrow.Xml.Macro;

using Tango.JobRunner;

namespace ScriptRunner
{
    internal class Program
    {
        static Task Main(string[] args)
        {
            return ApplicationRunner.RunAsync(() => Run(args));
        }

        private static Task Run(string[] args)
        {
            var runner = new Runner();
            return runner.Run(args);
        }
    }
}