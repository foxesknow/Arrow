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
using Tango.Workbench.Filters;

namespace Tango.Workbench.Scripts
{
    /// <summary>
    /// Parses a script
    /// </summary>
    public sealed class Parser
    {
        private readonly IInstanceFactory m_Factory = InstanceFactory.New();

        private readonly RunnableFactory m_RunnableFactory;

        /// <summary>
        /// Initalizes the instance
        /// </summary>
        /// <param name="runnableFactory">The factory to use for creating jobs</param>
        /// <exception cref="ArgumentNullException"></exception>
        public Parser(RunnableFactory runnableFactory)
        {
            if(runnableFactory is null) throw new ArgumentNullException(nameof(runnableFactory));

            m_RunnableFactory = runnableFactory;
            m_RunnableFactory.RegisterJob("Pipeline", typeof(PipelineJob));
            m_RunnableFactory.RegisterFilter("Tee", typeof(TeeFilter));
        }

        /// <summary>
        /// True to turn on verbosity in all jobs, regardless of what the jobs say
        /// </summary>
        public bool Verbose{get; init;}

        /// <summary>
        /// Parses an xml script and populates the run sheet
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="scriptElement"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Parse(Batch batch, XmlElement scriptElement)
        {
            if(batch is null) throw new ArgumentNullException(nameof(batch));
            if(scriptElement is null) throw new ArgumentNullException(nameof(scriptElement));

            m_Factory.ApplyNodeAttributes(scriptElement, batch);

            foreach(XmlElement element in scriptElement.SelectNodesOrEmpty("*"))
            {
                var what = element.LocalName;

                switch(what)
                {
                    case "Group":
                        HandleGroup(batch, element);
                        break;

                    case "Import":
                        HandleImport(batch, element);
                        break;

                    default: break;
                }
            }
        }

        private void HandleGroup(Batch batch, XmlElement groupElement)
        {
            var transactional = ParseFlag(groupElement, "transactional", "true");
            var enabled = ParseFlag(groupElement, "enabled", "true");
            var allowFail = ParseFlag(groupElement, "allowFail", "false");
            var verbose = ParseFlag(groupElement, "verbose", "false");

            var group = new Group(batch)
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
                
                var job = m_RunnableFactory.MakeJob(jobName);
                XmlCreation.Apply(job, jobElement);
                if(job.Name is null) job.Name = jobName;
                
                // Work out and set the appropriate verbosity
                job.Verbose |= (verbose | this.Verbose);

                var logName = MakeLogName(group.Name, job.Name);
                job.SetLogName(logName);

                if(job is PipelineJob pipelineJob)
                {
                    ComposePipeline(group, logName, pipelineJob, jobElement);
                }

                group.Jobs.Add(job);
            }

            batch.Add(group);
        }

        private void HandleImport(Batch batch, XmlElement importElement)
        {
            var assemblyName = importElement.InnerText.Trim();
            if(string.IsNullOrWhiteSpace(assemblyName)) throw new WorkbenchException($"no assembly specified in import element");

            var assembly = TypeResolver.LoadAssembly(assemblyName);
            m_RunnableFactory.Register(assembly);
        }

        private void ComposePipeline(Group group, string rootLogName, PipelineJob pipeline, XmlNode pipelineElement)
        {
            var parts = pipelineElement.SelectNodesOrEmpty("*")
                                       .Cast<XmlElement>()
                                       .ToArray();

            if(parts.Length == 0) throw new WorkbenchException("a pipeline needs at least a source");

            // The first item in the pipeline is the source, the rest are filters
            var source = m_RunnableFactory.MakeSource(parts[0].Name);
            XmlCreation.Apply(source, parts[0]);
            if(source.Name is null) source.Name = parts[0].Name;
            source.Verbose |= pipeline.Verbose;

            source.SetLogName(MakeLogName(rootLogName, source.Name));
            
            pipeline.Source = source;

            for(int i = 1; i < parts.Length; i++)
            {
                var filter = m_RunnableFactory.MakeFilter(parts[i].Name);
                XmlCreation.Apply(filter, parts[i]);
                if(filter.Name is null) filter.Name = parts[i].Name;
                filter.Verbose |= pipeline.Verbose;

                var filterLogName = MakeLogName(rootLogName, filter.Name);
                filter.SetLogName(filterLogName);

                if(filter is TeeFilter teeFilter)
                {
                    ComposeTee(filterLogName, teeFilter, parts[i]);
                }
                
                pipeline.Filters.Add(filter);
            }
        }

        private void ComposeTee(string rootLogName, TeeFilter teeFilter, XmlElement teeElement)
        {
            var parts = teeElement.SelectNodesOrEmpty("*")
                                       .Cast<XmlElement>()
                                       .ToArray();

            if(parts.Length == 0) throw new WorkbenchException("a tee needs at least one filter");

            foreach(var filterElement in parts)
            {
                var filter = m_RunnableFactory.MakeFilter(filterElement.Name);
                XmlCreation.Apply(filter, filterElement);
                if(filter.Name is null) filter.Name = filterElement.Name;
                filter.Verbose |= teeFilter.Verbose;

                var filterLogName = MakeLogName(rootLogName, filter.Name);
                filter.SetLogName(filterLogName);

                if(filter is TeeFilter subFilter)
                {
                    ComposeTee(filterLogName, subFilter, filterElement);
                }

                teeFilter.Filters.Add(filter);
            }
        }

        private bool ParseFlag(XmlElement element, string name, string defaultValue)
        {
            var flag = element.Attributes.GetValueOrDefault(name, defaultValue);
            return bool.Parse(flag);
        }

        private static string MakeLogName(string groupName, string? jobName)
        {
            if(jobName is null) jobName = "No name";

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
