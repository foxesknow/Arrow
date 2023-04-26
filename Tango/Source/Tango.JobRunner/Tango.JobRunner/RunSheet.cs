using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.JobRunner
{
    public abstract class RunSheet
    {
        public abstract void Add(Group group);

        public abstract Task<IReadOnlyList<Scorecard>> Run(RunConfig runData);

        public abstract JobContext MakeContext(Group group);
    }
}
