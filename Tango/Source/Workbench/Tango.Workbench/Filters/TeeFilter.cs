using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Channels;

namespace Tango.Workbench.Filters
{   
    /// <summary>
    /// The tee filter allows you to branch off from a pipeline flow and do additional processing on 
    /// the data flowing through the pipeline
    /// </summary>
    internal sealed class TeeFilter : Filter
    {
        public override async IAsyncEnumerable<object> Run(IAsyncEnumerable<object> items)
        {
            var channel = Channel.CreateUnbounded<object>();
            var reader = channel.Reader;
            var writer = channel.Writer;

            var factory = BuildPipelineFactory();
            var teeTask = Task.Run(() => RunTee(reader.ReadAllAsync(), factory));

            try
            {
                

                await foreach(var item in items)
                {
                    // We need to pass through what we're receiving...
                    yield return item;

                    // ...as well as pass it into the tee bit of our filter
                    await writer.WriteAsync(item);
                }
            }
            finally
            {
                writer.Complete();
                await teeTask;    
            }
        }

        internal override void RegisterRuntimeDependencies(RuntimeDependencies dependencies)
        {
            base.RegisterRuntimeDependencies(dependencies);

            foreach(var component in this.Filters)
            {
                component.RegisterRuntimeDependencies(dependencies);
            }
        }

        internal override void UnregisterRuntimeDependencies()
        {
            base.UnregisterRuntimeDependencies();

            foreach(var component in this.Filters)
            {
                component.UnregisterRuntimeDependencies();
            }
        }

        private async Task RunTee(IAsyncEnumerable<object> input, Func<IAsyncEnumerable<object>, IAsyncEnumerable<object>> function)
        {
            var sequence = function(input);

            long count = 0;
            try
            {                
                await foreach(var item in sequence)
                {
                    count++;
                }
            }
            finally
            {
                VerboseLog.Info($"{count} items went through the tee");
            }
        }

        private Func<IAsyncEnumerable<object>, IAsyncEnumerable<object>> BuildPipelineFactory()
        {
            Func<IAsyncEnumerable<object>, IAsyncEnumerable<object>> function = input => this.Filters[0].Run(input);

            foreach(var filter in this.Filters.Skip(1))
            {
                var first = function;
                Func<IAsyncEnumerable<object>, IAsyncEnumerable<object>> next = input => filter.Run(first(input));
                function = next;
            }

            return function;
        }

        /// <summary>
        /// Filters to apply to the data flowing through the pipeline
        /// </summary>
        internal List<Filter> Filters{get; set;} = new();
    }
}
