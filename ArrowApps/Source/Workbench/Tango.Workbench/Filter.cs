using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tango.Workbench.Data;

namespace Tango.Workbench
{
    /// <summary>
    /// Defines a filter for a pipeline
    /// </summary>
    public abstract class Filter : Runnable
    {
        /// <summary>
        /// Processes the incoming data and generates any outgoing data.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public abstract IAsyncEnumerable<object> Run(IAsyncEnumerable<object> items);

        /// <summary>
        /// Returns a read-only structured object for a given object.
        /// If the object is already a structured object then it is returned.
        /// </summary>
        /// <param name="object"></param>
        /// <returns></returns>
        protected IReadOnlyStructuredObject ToStructuredObject(object @object)
        {
            return @object switch
            {
                StructuredObject s  => s,
                var other           => StructuredObject.From(other)
            };
        }
    }
}
