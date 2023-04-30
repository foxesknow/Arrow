﻿using System;
using System.Threading.Tasks;
using System.Xml;

using Arrow.Application;
using Arrow.Execution;
using Arrow.Reflection;

[assembly: ForceAssemblyReference(typeof(Tango.Workbench.Python.PythonJob))]

namespace ScriptRunner
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await ApplicationRunner.RunAsync(() => Run(args));

            #if DEBUG
                if(System.Diagnostics.Debugger.IsAttached)
                {
                    Console.ReadLine();
                }
            #endif
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