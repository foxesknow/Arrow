using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Workbench
{
    sealed class RuntimeDependencies
    {
        public RuntimeDependencies(JobContext context, Score score)
        {
            this.Context = context;
            this.Score = score;
        }

        public JobContext Context{get;}

        public Score Score{get;}
    }
}
