using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

using Arrow.Application;
using Arrow.Calendar;
using Arrow.Calendar.ClockDrivers;
using Arrow.Configuration;
using Arrow.IO;
using Arrow.Logging;
using Arrow.Storage;
using Arrow.Text;
using Arrow.Threading.Tasks;
using Arrow.Xml.Macro;

using Tango.Workbench;
using Tango.Workbench.Jobs;
using Tango.Workbench.Scripts;
using System.Text;

namespace Workbench
{
    internal class Runner : IDisposable
    {
        private static readonly ILog Log = new PrefixLog(LogManager.GetDefaultLog(), "[Runner]");

        private string? m_ScriptFilename;
        private RunConfig m_RunConfig = RunConfig.All();
        private DateTime? m_BaselineDate;
        private bool m_Live = false;
        private bool m_Verbose = false;
        private bool m_DumpScript;

        private PidFile? m_PidFile;

        public void Dispose()
        {
            m_PidFile?.Dispose();
        }

        public Task Run(string[] args)
        {
            ParseCommandLine(args);

            if(m_ScriptFilename is null) throw new WorkbenchException("no script specified");

            if(m_BaselineDate.HasValue) ChangeClock(m_BaselineDate.Value);

            if(m_Live)
            {
                Log.Info("RUNNING IN LIVE MODE");
            }
            else
            {
                Log.Info("Running in test mode");
            }

            var batch = MakeBatch();

            var runnableFactory = MakeRunnableFactory();
            var parser = new Parser(runnableFactory)
            {
                Verbose = m_Verbose
            };

            var (script, uri) = LoadScript(m_ScriptFilename);
            if(m_DumpScript) LogDocument(script);

            parser.Parse(batch, script.DocumentElement!);

            if(uri.LocalPath is not null)
            {
                batch.ScriptDirectory = Path.GetDirectoryName(uri.LocalPath) ?? "";
            }

            return Execute(batch);
        }

        private async Task Execute(RunnerBatch batch)
        {
            try
            {
                Log.Info("Running sheet");

                await batch.Run(m_RunConfig).ContinueOnAnyContext();
            }
            catch(Exception e)
            {
                Log.Error("Failed to run sheet", e);
            }
        }

        private RunnableFactory MakeRunnableFactory()
        {
            if(m_Live)
            {
                return new RunnableFactory();
            }

            return new RunnableFactory(static type => RunnableFactory.MakeMockComponent(type));
        }

        private RunnerBatch MakeBatch()
        {
            var batch = AppConfig.GetSectionObject<RunnerBatch>("App", "Batch");
            return batch;
        }

        private void ChangeClock(DateTime when)
        {
            var clockDriver = new BaselineClockDriver(when);
            GlobalClockDriverManager.Install(clockDriver);

            Log.Info($"Installed a baseline clock for {when}");
        }

        private static (XmlDocument Document, Uri Location) LoadScript(string filename)
        {
            var expandedFilename = TokenExpander.ExpandText(filename);
            var uri = Accessor.CreateUri(expandedFilename);

            var expander = new XmlMacroExpander();
            var scriptDocument = expander.Expand(uri);

            return (scriptDocument, uri);
        }

        private void ParseCommandLine(string[] args)
        {
            foreach(var arg in args)
            {
                if(CommandLineSwitch.TryParse(arg, out var command) == false)
                {
                    // Assume it's the name of the script to run
                    m_ScriptFilename = arg;
                    continue;
                }

                switch(command.Name.ToLower())
                {
                    case "runfrom":
                        command.EnsureValuePresent();
                        m_RunConfig = RunConfig.From(command.Value!);
                        break;

                    case "runonly":
                        command.EnsureValuePresent();
                        m_RunConfig = RunConfig.Single(command.Value!);
                        break;

                    case "runall":
                        command.EnsureNoValuePresent();
                        m_RunConfig = RunConfig.All();
                        break;

                    case "date":
                        command.EnsureValuePresent();
                        m_BaselineDate = ParseDateTime(command.Value!);
                        break;

                    case "live":
                        command.EnsureNoValuePresent();
                        m_Live = true;
                        break;

                    case "v":
                    case "verbose":
                        command.EnsureNoValuePresent();
                        m_Verbose = true;
                        break;

                    case "pidfile":
                        command.EnsureValuePresent();
                        m_PidFile = new PidFile(command.Value!);
                        break;

                    case "dumpscript":
                        command.EnsureNoValuePresent();
                        m_DumpScript = true;

                        break;

                    default:
                        break;
                }
            }
        }

        private DateTime ParseDateTime(string date)
        {
            var now = Clock.Now;

            var when = DateTime.ParseExact(date, "yyyyMMdd", null);
            var local = DateTime.SpecifyKind(when, DateTimeKind.Local).Add(now.TimeOfDay);
            
            return local;
        }

        private void LogDocument(XmlDocument document)
        {
            var settings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "  "
            };

            var b = new StringBuilder();

            using(var writer = XmlWriter.Create(b, settings))
            {
                document.Save(writer);
            }

            Log.Info(b.ToString());
        }
    }
}
