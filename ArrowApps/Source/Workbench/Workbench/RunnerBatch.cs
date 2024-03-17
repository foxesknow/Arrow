using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Data;
using Arrow.Data.DatabaseManagers;
using Arrow.Logging;
using Arrow.Threading.Tasks;

using Tango.Workbench;

namespace Workbench
{
    internal class RunnerBatch : Batch
    {
        public string ScriptDirectory{get; set;} = "";

        /// <summary>
        /// The database manager which will be ultimately used by the jobs
        /// </summary>
        public XmlDatabaseManager DatabaseManager{get; set;} = new();

        /// <summary>
        /// True to send a report when the jobs have run
        /// </summary>
        public bool SendReport{get; set;} = true;

        public override JobContext MakeContext(Group group)
        {
            var context = new RunnerJobContext(this)
            {
                UseTransactions = group.Transactional,
            };

            context.SetScriptDirectory(this.ScriptDirectory);

            return context;
        }

        
    }
}
