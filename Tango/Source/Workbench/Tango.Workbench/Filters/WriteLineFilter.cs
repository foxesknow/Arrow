using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Workbench.Filters
{
    /// <summary>
    /// Writes each item passing through the filter to the console
    /// </summary>
    [Filter("WriteLine")]
    public sealed class WriteLineFilter : Filter
    {
        public override async IAsyncEnumerable<object> Run(IAsyncEnumerable<object> items)
        {
            var writer = (this.AsError ? Console.Error : Console.Out);
            var prefix = this.Prefix;

            await foreach(var item in items)
            {
                if(prefix is null)
                {
                    await writer.WriteLineAsync(item.ToString());
                }
                else
                {
                    await writer.WriteLineAsync($"{prefix}{item}");
                }

                yield return item;
            }
        }

        /// <summary>
        /// True to write to stderr, false to write to stdout
        /// </summary>
        public bool AsError{get; set;}

        /// <summary>
        /// An optional prefix to add to the output
        /// </summary>
        public string? Prefix{get; set;}
    }
}
