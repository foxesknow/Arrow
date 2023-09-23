using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Channels;

using Arrow.Threading.Tasks;

namespace Tango.Workbench
{
    /// <summary>
    /// The tee filter allows you to branch off from a pipeline flow and do additional processing on 
    /// the data flowing through the pipeline
    /// </summary>
    internal sealed class TeeFilter : Filter
    {
        public override async IAsyncEnumerable<object> Run(IAsyncEnumerable<object> items)
        {
            var ct = this.Context.CancellationToken;

            var channel = Channel.CreateUnbounded<object>();
            var reader = channel.Reader;
            var writer = channel.Writer;

            var factory = BuildPipelineFactory();
            var teeTask = Task.Run(() => RunTee(reader.ReadAllAsync(ct), factory));

            try
            {
                await foreach(var item in items.WithCancellation(this.Context.CancellationToken))
                {
                    ct.ThrowIfCancellationRequested();

                    // We need to pass through what we're receiving...
                    yield return item;

                    // ...as well as pass it into the tee bit of our filter
                    if(teeTask.IsCompleted == false) await writer.WriteAsync(item, ct);
                }
            }
            finally
            {
                writer.Complete();
                await teeTask;
            }
        }

        private async Task RunTee(IAsyncEnumerable<object> input, Func<IAsyncEnumerable<object>, IAsyncEnumerable<object>> function)
        {
            long count = 0;

            try
            {
                // We need to tell the context that we've entered a new async scope.
                // Some things, like database connections can't be shared amongst threads
                // so this gives the framework chance to make any adjustments
                this.Context.EnterNewAsyncScope();

                var sequence = function(input);

                var ct = this.Context.CancellationToken;

                await foreach(var item in sequence.WithCancellation(ct).ContinueOnAnyContext())
                {
                    count++;
                    ct.ThrowIfCancellationRequested();
                }
            }
            catch(Exception e)
            {
                // Once reason to end up here is that the one of the filters requested a cancel after
                // a period of time. The AllowFail setting will allow us to decide if this should ripple
                // up the run stack to ultimately cancel the entire group

                if(AllowFail)
                {
                    Log.Warn("Tee failed, but this is allowed", e);
                    Score.ReportWarning($"Tee failed, but this is allowed. Message = {e.Message}");
                }
                else
                {
                    Log.Error("Tee failed", e);
                    Score.ReportError($"Tee failed. Message = {e.Message}");
                    throw;
                }
            }
            finally
            {
                VerboseLog.Info($"{count} items went through the tee");
                this.Context.LeaveAsyncScope();
            }
        }

        internal override void RegisterRuntimeDependencies(RuntimeDependencies dependencies)
        {
            base.RegisterRuntimeDependencies(dependencies);

            foreach(var component in Filters)
            {
                component.RegisterRuntimeDependencies(dependencies);
            }
        }

        internal override void UnregisterRuntimeDependencies()
        {            
            foreach(var component in Filters)
            {
                component.UnregisterRuntimeDependencies();
            }

            base.UnregisterRuntimeDependencies();
        }

        private Func<IAsyncEnumerable<object>, IAsyncEnumerable<object>> BuildPipelineFactory()
        {
            Func<IAsyncEnumerable<object>, IAsyncEnumerable<object>> function = input => Filters[0].Run(input);

            foreach(var filter in Filters.Skip(1))
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

        /// <summary>
        /// True if a tee is allowed to fail. 
        /// </summary>
        public bool AllowFail{get; set;}
    }
}
