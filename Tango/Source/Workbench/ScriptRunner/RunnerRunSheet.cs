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

namespace ScriptRunner
{
    internal class RunnerRunSheet : RunSheet
    {
        private static readonly ILog Log = new PrefixLog(LogManager.GetDefaultLog(), "[RunSheet]");

        private readonly List<Group> m_Groups = new();

        public override void Add(Group group)
        {
            if(group is null) throw new ArgumentNullException(nameof(group));

            m_Groups.Add(group);
        }

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

        public override async Task<IReadOnlyList<Scorecard>> Run(RunConfig runConfig)
        {
            var scorecards = new List<Scorecard>();

            var doneInitialFrom = false;

            foreach(var group in m_Groups)
            {
                var runGroup = false;

                switch(runConfig.RunMode)
                {
                    case  RunMode.Single:
                        runGroup = string.Equals(group.Name, runConfig.GroupName, StringComparison.OrdinalIgnoreCase);
                        break;

                    case RunMode.From:
                        if(doneInitialFrom)
                        {
                            runGroup = true;
                        }
                        else
                        {
                            runGroup = string.Equals(group.Name, runConfig.GroupName, StringComparison.OrdinalIgnoreCase);
                            doneInitialFrom = runGroup;
                        }
                        break;

                    default:
                    case RunMode.All:
                        runGroup = true;
                        break;
                }

                if(group.Enabled == false)
                {
                    Log.Info($"Group {group.Name} is disabled");
                    continue;
                }

                if(runGroup == false)
                {
                    Log.Info($" Not running group. Config = {runConfig}");
                    continue;
                }

                Log.Info($"Running {group.Name}");

                var (succeeded, scorecard) = await group.Run().ContinueOnAnyContext();
                scorecards.Add(scorecard);

                if(succeeded == false)
                {
                    // Oh no...
                    Environment.ExitCode = -1;
                    break;
                }
            }

            return scorecards;
        }
    }
}
