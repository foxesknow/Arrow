using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Collections;

namespace Tango.Workbench.Sources
{
    /// <summary>
    /// Generates a sequence of numbers
    /// </summary>
    [Source("Numbers")]
    public sealed class NumbersSource : Source
    {
        public override IAsyncEnumerable<object> Run()
        {
            var range = Enumerable.Range(this.Start, this.Count)
                                  .Cast<object>();

            return range.ToAsyncEnumerable();
        }

        /// <summary>
        /// The start number
        /// </summary>
        public int Start{get; set;}
        
        /// <summary>
        /// How many to generate
        /// </summary>
        public int Count{get; set;}
    }
}
