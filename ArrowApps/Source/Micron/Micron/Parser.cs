using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using Arrow.Xml;
using Arrow.Xml.ObjectCreation;
using Quartz;

namespace Micron
{
    internal class Parser
    {
        public IReadOnlyList<(IJobDetail Job, ITrigger Trigger)> Parse(XmlDocument document)
        {
            var schedules = new List<(IJobDetail Job, ITrigger Trigger)>();

            foreach(XmlNode element in document.DocumentElement!.SelectNodesOrEmpty("Run"))
            {
                var runData = XmlCreation.Create<RunJobData>(element);

                var job = JobBuilder.Create<RunJob>()
                                    .WithIdentity(runData.Name!)
                                    .UsingJobData(nameof(JobData.Name), runData.Name)
                                    .UsingJobData(nameof(RunJobData.App), runData.App)
                                    .UsingJobData(nameof(RunJobData.WorkingDirectory), runData.WorkingDirectory)
                                    .UsingJobData(nameof(RunJobData.Arguments), runData.Arguments)
                                    .Build();

                var trigger = MakeTrigger(runData);

                schedules.Add((job, trigger));
            }

            return schedules;
        }

        private ITrigger MakeTrigger(JobData jobData)
        {
            if(string.IsNullOrWhiteSpace(jobData.Cron) == false)
            {
                return TriggerBuilder.Create()
                                     .WithIdentity(jobData.Name)
                                     .WithCronSchedule(jobData.Cron)
                                     .Build();
            }

            if(jobData.Days == DaysOfTheWeek.None || jobData.At < TimeSpan.Zero)
            {
                throw new Exception($"invalid schedule for {jobData.Name}");
            }

            var at = jobData.At;

            return TriggerBuilder.Create()
                                 .WithIdentity(jobData.Name)
                                 .WithDailyTimeIntervalSchedule(x =>
                                     x.InTimeZone(TimeZoneInfo.Local)
                                      .WithIntervalInHours(24)
                                      .OnDaysOfTheWeek(jobData.Days.ToDayOfWeekList())
                                      .StartingDailyAt(TimeOfDay.HourMinuteAndSecondOfDay(at.Hours, at.Minutes, at.Seconds))
                                  ).Build();
        }
    }
}
