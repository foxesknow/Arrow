using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
