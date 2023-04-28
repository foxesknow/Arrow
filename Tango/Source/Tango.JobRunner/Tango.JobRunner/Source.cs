using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.JobRunner
{
    /// <summary>
    /// Defines a pipeline source
    /// </summary>
    public abstract class Source : Runnable
    {
        /// <summary>
        /// Generates the initial data for the pipeline
        /// </summary>
        /// <returns></returns>
        public abstract IAsyncEnumerable<object> Run();
    }
}
