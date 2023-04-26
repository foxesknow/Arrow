using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using Arrow.Reflection;
using Arrow.Xml;
using Arrow.Xml.ObjectCreation;

namespace Tango.JobRunner.Scripts
{
    public sealed class Parser
    {
        private readonly IInstanceFactory m_Factory = InstanceFactory.New();

        private readonly JobFactory m_JobFactory;

        public Parser(JobFactory jobFactory)
        {
            if(jobFactory is null) throw new ArgumentNullException(nameof(jobFactory));

            m_JobFactory = jobFactory;
        }

        public void Parse(RunSheet runSheet, XmlElement scriptElement)
        {
            if(runSheet is null) throw new ArgumentNullException(nameof(runSheet));
            if(scriptElement is null) throw new ArgumentNullException(nameof(scriptElement));

            m_Factory.ApplyNodeAttributes(scriptElement, runSheet);

            foreach(XmlElement element in scriptElement.SelectNodesOrEmpty("*"))
            {
                var what = element.LocalName;

                switch(what)
                {
                    case "Group":
                        HandleGroup(runSheet, element);
                        break;

                    case "Import":
                        HandleImport(runSheet, element);
                        break;

                    default: break;
                }
            }
        }

        private void HandleGroup(RunSheet runSheet, XmlElement groupElement)
        {
            var transactional = ParseFlag(groupElement, "transactional", "true");
            var enabled = ParseFlag(groupElement, "enabled", "true");
            var allowFail = ParseFlag(groupElement, "allowFail", "false");

            var group = new Group(runSheet)
            {
                Name = groupElement.Attributes.GetValueOrDefault("name", "No name"),
                Enabled = enabled,
                Transactional = transactional,
                AllowFail = allowFail
            };

            foreach(XmlElement jobElement in groupElement.SelectNodesOrEmpty("*"))
            {
                var jobName = jobElement.Name;
                
                var job = m_JobFactory.Make(jobName);
                XmlCreation.Apply(job, jobElement);

                var logName = MakeLogName(group.Name, (job.Name ?? jobName));
                job.SetLogName(logName);

                group.Jobs.Add(job);
            }

            runSheet.Add(group);
        }

        private void HandleImport(RunSheet runSheet, XmlElement importElement)
        {
            var assemblyName = importElement.InnerText.Trim();
            if(string.IsNullOrWhiteSpace(assemblyName)) throw new JobRunnerException($"no assembly specified in import element");

            var assembly = TypeResolver.LoadAssembly(assemblyName);
            m_JobFactory.Register(assembly);
        }

        private bool ParseFlag(XmlElement element, string name, string defaultValue)
        {
            var flag = element.Attributes.GetValueOrDefault(name, defaultValue);
            return bool.Parse(flag);
        }

        private static string MakeLogName(string groupName, string? jobName)
        {
            if(jobName is null) jobName = "No job name";

            return $"{groupName} : {jobName}";
        }
    }
}
