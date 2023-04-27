using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.JobRunner
{
    /// <summary>
    /// A run sheet describes the groups that will be run
    /// </summary>
    public abstract class RunSheet
    {
        /// <summary>
        /// Adds a new group to the sheet
        /// </summary>
        /// <param name="group"></param>
        public abstract void Add(Group group);

        /// <summary>
        /// Executes the sheet
        /// </summary>
        /// <param name="runData"></param>
        /// <returns></returns>
        public abstract Task<IReadOnlyList<Scorecard>> Run(RunConfig runData);

        /// <summary>
        /// Creates the job context that will be used by the jobs
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public abstract JobContext MakeContext(Group group);
    }
}
