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

        private static async Task Run(string[] args)
        {
            using(var runner = new Runner())
            {
                await runner.Run(args);
            }
        }
    }
}