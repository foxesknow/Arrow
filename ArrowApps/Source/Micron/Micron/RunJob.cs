using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Quartz;
using Quartz.Impl;

using Arrow.Logging;

namespace Micron
{
    /// <summary>
    /// Run an application
    /// </summary>
    internal class RunJob : IJob
    {
        private static readonly ILog Log = new PrefixLog(LogManager.GetDefaultLog(), "[RunJob]");

        public Task Execute(IJobExecutionContext context)
        {
            var map = context.MergedJobDataMap;

            var name = map.GetString(nameof(RunJobData.Name));
            Log.Info($"Starting {name}");

            var app = map.GetString(nameof(RunJobData.App));
            var arguments = map.GetString(nameof(RunJobData.Arguments));
            var workingDirectory = map.GetString(nameof(RunJobData.WorkingDirectory));

            var startInfo = new ProcessStartInfo(app!)
            {
                Arguments = arguments,
                WorkingDirectory = workingDirectory,
                UseShellExecute = false
            };

            try
            {
                var process = Process.Start(startInfo);            
                if(process is not null)
                {
                    Log.Info($"started {name} with PID {process.Id}");
                }
                else
                {
                    Log.Error($"failed to start {name}");
                }
            }
            catch(Exception e)
            {
                Log.Error($"failed to start {name}", e);
            }

            return Task.CompletedTask;

        }
    }
}
