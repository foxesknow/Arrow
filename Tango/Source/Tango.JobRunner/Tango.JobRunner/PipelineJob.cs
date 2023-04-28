using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.JobRunner
{
    /// <summary>
    /// Implements the pipeline functionality
    /// </summary>
    internal sealed class PipelineJob : Job
    {
        public override async ValueTask Run()
        {
            long count = 0;

            try
            {
                RegisterComponentRequirements();

                var factory = BuildPipelineFactory();
                var sequence = factory();

                await foreach(var item in sequence)
                {
                    count++;
                }
            }
            finally
            {
                VerboseLog.Info($"{count} items went through the pipeline");

                UnregisterComponentRequirements();
            }
        }

        private void RegisterComponentRequirements()
        {
            foreach(var component in AllComponents())
            {
                component.Context = this.Context;
                component.Score = this.Score;
            }
        }

        private void UnregisterComponentRequirements()
        {
            foreach(var component in AllComponents())
            {
                component.Context = null!;
                component.Score = null!;
            }
        }

        private Func<IAsyncEnumerable<object>> BuildPipelineFactory()
        {
            var function = () => this.Source.Run();

            foreach(var filter in this.Filters)
            {
                var first = function;
                var next = () => filter.Run(first());
                function = next;
            }

            return function;
        }

        private IEnumerable<Runnable> AllComponents()
        {
            return this.Filters.Cast<Runnable>()
                               .Prepend(this.Source);
        }

        /// <summary>
        /// The source of data for the pipeline
        /// </summary>
        internal Source Source{get; set;} = null!;

        /// <summary>
        /// Filters to apply to the data flowing through the pipeline
        /// </summary>
        internal List<Filter> Filters{get; set;} = new();
    }
}
