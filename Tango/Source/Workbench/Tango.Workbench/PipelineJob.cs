using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Workbench
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
                var factory = BuildPipelineFactory();
                var sequence = factory();

                var ct = this.Context.CancellationToken;

                await foreach(var item in sequence.WithCancellation(this.Context.CancellationToken))
                {
                    ct.ThrowIfCancellationRequested();
                    count++;
                }
            }
            finally
            {
                VerboseLog.Info($"{count} items went through the pipeline");
            }
        }

        internal override void RegisterRuntimeDependencies(RuntimeDependencies dependencies)
        {
            base.RegisterRuntimeDependencies(dependencies);

            foreach(var component in AllComponents())
            {
                component.RegisterRuntimeDependencies(dependencies);
            }
        }

        internal override void UnregisterRuntimeDependencies()
        {
            foreach(var component in AllComponents())
            {
                component.UnregisterRuntimeDependencies();
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
