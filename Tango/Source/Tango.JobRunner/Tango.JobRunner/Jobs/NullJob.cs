using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Workbench.Jobs
{
    /// <summary>
    /// Does nothing
    /// </summary>
    [Job("Null")]
    public sealed class NullJob : Job
    {
        public override ValueTask Run()
        {
            return default;
        }
    }
}
