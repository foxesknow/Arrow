using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Workbench
{
    public enum RunMode
    {
        /// <summary>
        /// Run all groups
        /// </summary>
        All,

        /// <summary>
        /// Run a single group
        /// </summary>
        Single,

        /// <summary>
        /// Run from a given group to the end of the script
        /// </summary>
        From
    }
}
