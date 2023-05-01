using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using Quartz;

using Arrow.Application.DaemonHosting;
using Quartz.Impl;

using Arrow.Logging;
using Arrow.Text;
using Arrow.Storage;
using Arrow.Xml.Macro;

namespace Micron
{
    internal class Daemon : DaemonBase
    {
        private static readonly ILog Log = new PrefixLog(LogManager.GetDefaultLog(), "[Daemon]");

        private readonly List<(string Filename, IScheduler Scheduler)> m_Schedulers = new();

        protected override async ValueTask StartDaemon(string[] args)
        {
            foreach(var filename in args)
            {
                Log.Info($"loading {filename}");
                var scheduler = await MakeScheduler(filename);
                m_Schedulers.Add((filename, scheduler));
            }

            try
            {
                foreach(var schedules in m_Schedulers)
                {
                    Log.Info($"starting {schedules.Filename}");
                    await schedules.Scheduler.Start();
                }
            }
            catch(Exception e)
            {
                Log.Error("failed to start", e);
                await StopAll();

                throw;
            }
        }

        protected override async ValueTask StopDaemon()
        {
            await StopAll();
        }

        private async ValueTask StopAll()
        {
            foreach(var schedules in m_Schedulers)
            {
                Log.Info($"stopping {schedules.Filename}");
                await schedules.Scheduler.Shutdown();
            }

            m_Schedulers.Clear();
        }

        private async ValueTask<IScheduler> MakeScheduler(string filename)
        {
            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            var document = LoadScript(filename);

            var parser = new Parser();
            var schedules = parser.Parse(document);

            foreach(var schedule in schedules)
            {
                await scheduler.ScheduleJob(schedule.Job, schedule.Trigger);
            }

            return scheduler;
        }

        private XmlDocument LoadScript(string filename)
        {
            var expandedFilename = TokenExpander.ExpandText(filename);
            var uri = Accessor.CreateUri(expandedFilename);

            var expander = new XmlMacroExpander();
            var scriptDocument = expander.Expand(uri);

            return scriptDocument;
        }
    }
}
