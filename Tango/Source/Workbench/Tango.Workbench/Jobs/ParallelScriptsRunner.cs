using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using System.Xml;
using System.Threading;

namespace Tango.Workbench.Jobs
{
    /// <summary>
    /// Runs multiple scripts at once by invoking new instance of the host application
    /// </summary>
    [Job("ParallelScripts")]
    public sealed class ParallelScriptsRunner : Job
    {
        public override ValueTask Run()
        {
            var scripts = ExpandScripts();
            if(scripts.Count == 0) return default;

            using(var finished = new ManualResetEvent(false))
            using(var pool = new Semaphore(this.Instances, this.Instances))
            {
                long numberOfScripts = scripts.Count;

                var syncRoot = new object();
                var completedProcesses = new List<Process>();
                
                var failedScripts = new List<string>();

                foreach(var script in scripts)
                {
                    // Wait for availability
                    pool.WaitOne();
                    
                    // Free up any resources
                    DisposeProcesses(syncRoot, completedProcesses);

                    var process = new Process()
                    {
                        StartInfo = MakeProcessStartInfo(script),
                        EnableRaisingEvents = true
                    };

                    process.Exited += (sender, _) =>
                    {
                        pool.Release(1);

                        lock(syncRoot)
                        {
                            var process = (Process)sender!;
                            if(process.ExitCode == 0)
                            {
                                Log.Info($"Completed {script}");
                            }
                            else
                            {
                                Log.Error($"Failed {script} with exit code {process.ExitCode}");
                                failedScripts.Add(script);
                            }

                            completedProcesses.Add(process);

                            if(Interlocked.Decrement(ref numberOfScripts) == 0)
                            {
                                finished.Set();
                            }
                        }
                    };

                    Log.Info($"Starting {script}");
                    process.Start();
                    Log.Info($"Started {script} with PID = {process.Id}");
                }

                // Now just wait for everything to complete
                Log.Info("Waiting for scripts to complete");
                finished.WaitOne();
                Log.Info("All scripts to complete");

                DisposeProcesses(syncRoot, completedProcesses);

                if(failedScripts.Count != 0)
                {
                    foreach(var script in failedScripts)
                    {
                        this.Score.ReportError($"{script} failed");
                    }

                    if(this.AllowFail)
                    {
                        Log.Info("There were script failures, but AllowFail is set to true");
                    }
                    else
                    {
                        throw new WorkbenchException($"{failedScripts.Count} failed");
                    }
                }
            }

            return default;
        }

        private IReadOnlyList<string> ExpandScripts()
        {
            var expander = MakeExpander();
            return this.Scripts.Select(name => expander.Expand(name)).ToList();
        }

        private void DisposeProcesses(object syncRoot, List<Process> processes)
        {
            lock(syncRoot)
            {
                foreach(var process in processes)
                {
                    process.Dispose();
                }

                processes.Clear();
            }
        }
    
        private ProcessStartInfo MakeProcessStartInfo(string scriptFilename)
        {
            var exe = GetExecutable();

            var info = new ProcessStartInfo()
            {
                FileName = exe,
                WorkingDirectory = Path.GetDirectoryName(exe),
                Arguments = MakeCommandLine(scriptFilename),
                UseShellExecute = false
            };

            return info;
        }

        private string MakeCommandLine(string scriptFilename)
        {
            var b = new StringBuilder();

            b.AppendFormat("{0} ", Quote("/live"));

            if(string.IsNullOrWhiteSpace(this.Date) == false) b.AppendFormat("{0} ", Quote("/date:{0}", this.Date));
            if(string.IsNullOrWhiteSpace(this.RunFrom) == false) b.AppendFormat("{0} ", Quote("/runfrom:{0}", this.RunFrom));
            if(string.IsNullOrWhiteSpace(this.RunOnly) == false) b.AppendFormat("{0} ", Quote("/runonly:{0}", this.RunOnly));

            b.Append(Quote(scriptFilename));

            return b.ToString();

            static string Quote(string format, params object[] args)
            {
                return string.Concat("\"", string.Format(format, args), "\"");
            }
        }

        /// <summary>
        /// Works out which executable is running us!
        /// </summary>
        /// <returns></returns>
        /// <exception cref="WorkbenchException"></exception>
        private string GetExecutable()
        {
            var exe = Process.GetCurrentProcess().MainModule?.FileName;
            if(exe is null) throw new WorkbenchException($"could not work out what the executable is");

            return exe;
        }

        private Expander MakeScriptExpander()
        {
            var expander = MakeExpander();

            if(this.RelativeFilename is not null)
            {
                var uri = new Uri(this.RelativeFilename);
                var directory = Path.GetDirectoryName(uri.LocalPath);
                expander.Add("relativeDir", directory);
            }

            return expander;
        }

        /// <summary>
        /// The name of a file that will be used to set the $(relativeDir) expansion variable.
        /// This allows the script to refererence other scripts relative to where it is.
        /// 
        /// If you set it to:
        /// 
        ///     relativeFilename = "@{cfile}"
        ///     
        /// then this will be set by the xml macro expander to be the name of the script which
        /// the job belongs to.
        /// </summary>
        public string? RelativeFilename{get; set;}

        /// <summary>
        /// How many instances to run in parallel
        /// </summary>
        public int Instances{get; set;} = 4;

        /// <summary>
        /// The value to pass to the /date command line parameter
        /// </summary>
        public string? Date{get; set;}

        /// <summary>
        /// The value to pass to the /runfrom command line parameter
        /// </summary>
        public string? RunFrom{get; set;}

        /// <summary>
        /// The value to pass to the /runonly command line parameter
        /// </summary>
        public string? RunOnly{get; set;}

        /// <summary>
        /// The scripts to run
        /// </summary>
        public List<string> Scripts{get;} = new();

        /// <summary>
        /// True to allow the scripts to fail without causing the job to fail
        /// </summary>
        public bool AllowFail{get; set;}
    }
}
