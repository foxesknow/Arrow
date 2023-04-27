using System;
using System.Threading.Tasks;
using System.Xml;

using Arrow.Application;
using Arrow.Execution;
using Arrow.Reflection;

[assembly: ForceAssemblyReference(typeof(Tango.JobRunner.Python.PythonJob))]

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