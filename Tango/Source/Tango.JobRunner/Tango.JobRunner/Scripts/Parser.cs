using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using Arrow.Reflection;
using Arrow.Xml;
using Arrow.Xml.ObjectCreation;
using System.Text.Json;

namespace Tango.JobRunner.Scripts
{
    /// <summary>
    /// Parses a script
    /// </summary>
    public sealed class Parser
    {
        private readonly IInstanceFactory m_Factory = InstanceFactory.New();

        private readonly JobFactory m_JobFactory;

        /// <summary>
        /// Initalizes the instance
        /// </summary>
        /// <param name="jobFactory">The factory to use for creating jobs</param>
        /// <exception cref="ArgumentNullException"></exception>
        public Parser(JobFactory jobFactory)
        {
            if(jobFactory is null) throw new ArgumentNullException(nameof(jobFactory));

            m_JobFactory = jobFactory;
            m_JobFactory.RegisterJob("Pipeline", typeof(PipelineJob));
        }

        /// <summary>
        /// True to turn on verbosity in all jobs, regardless of what the jobs say
        /// </summary>
        public bool Verbose{get; init;}

        /// <summary>
        /// Parses an xml script and populates the run sheet
        /// </summary>
        /// <param name="runSheet"></param>
        /// <param name="scriptElement"></param>
        /// <exception cref="ArgumentNullException"></exception>
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
            var verbose = ParseFlag(groupElement, "verbose", "false");

            var group = new Group(runSheet)
            {
                Name = groupElement.Attributes.GetValueOrDefault("name", "No name"),
                Enabled = enabled,
                Transactional = transactional,
                AllowFail = allowFail,
                Verbose = verbose
            };

            foreach(XmlElement jobElement in groupElement.SelectNodesOrEmpty("*"))
            {
                var jobName = jobElement.Name;
                
                var job = m_JobFactory.MakeJob(jobName);
                XmlCreation.Apply(job, jobElement);
                if(job.Name is null) job.Name = jobName;
                
                // Work out and set the appropriate verbosity
                job.Verbose |= (verbose | this.Verbose);

                var logName = MakeLogName(group.Name, job.Name);
                job.SetLogName(logName);

                if(job is PipelineJob pipelineJob)
                {
                    ComposePipeline(group, pipelineJob, jobElement);
                }

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

        private void ComposePipeline(Group group, PipelineJob pipeline, XmlNode pipelineElement)
        {
            var parts = pipelineElement.SelectNodesOrEmpty("*")
                                       .Cast<XmlElement>()
                                       .ToArray();

            if(parts.Length == 0) throw new JobRunnerException("a pipeline needs at least a source");

            // The first item in the pipeline is the source, the rest are filters
            var source = m_JobFactory.MakeSource(parts[0].Name);
            XmlCreation.Apply(source, parts[0]);
            if(source.Name is null) source.Name = parts[0].Name;
            source.Verbose |= pipeline.Verbose;
            source.SetLogName(MakeLogName(group.Name, pipeline.Name, source.Name));
            
            pipeline.Source = source;

            for(int i = 1; i < parts.Length; i++)
            {
                var filter = m_JobFactory.MakeFilter(parts[i].Name);
                XmlCreation.Apply(filter, parts[i]);
                if(filter.Name is null) filter.Name = parts[i].Name;
                filter.Verbose |= pipeline.Verbose;
                filter.SetLogName(MakeLogName(group.Name, pipeline.Name, filter.Name));
                
                pipeline.Filters.Add(filter);
            }
        }

        private bool ParseFlag(XmlElement element, string name, string defaultValue)
        {
            var flag = element.Attributes.GetValueOrDefault(name, defaultValue);
            return bool.Parse(flag);
        }

        private static string MakeLogName(string groupName, string? jobName)
        {
            if(jobName is null) jobName = "No job name";

            return $"{groupName}:{jobName}";
        }

        private static string MakeLogName(string groupName, string? jobName, string? pipelineItem)
        {
            var name = MakeLogName(groupName, jobName);

            if(pipelineItem is null) pipelineItem = "No item name";
            return $"{name}:{pipelineItem}";
        }
    }
}
