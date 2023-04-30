using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Logging;
using Arrow.Threading;
using Arrow.Threading.Tasks;

namespace Tango.Workbench
{
    /// <summary>
    /// A group is a collection of jobs
    /// </summary>
    public sealed class Group
    {
        public Group(Batch script)
        {
            if(script is null) throw new ArgumentNullException(nameof(script));

            this.Script = script;
        }

        public Batch Script{get;}

        /// <summary>
        /// The name of the group
        /// </summary>
        public string Name{get; init;} = "No name";

        /// <summary>
        /// If the group is enabled.
        /// Only enabled groups are run
        /// </summary>
        public bool Enabled{get; init;} = true;

        /// <summary>
        /// True if a group is allowed to fail. 
        /// If so then this won't stop the next group from running
        /// </summary>
        public bool AllowFail{get; set;}

        /// <summary>
        /// True to make all database connections within the group,
        /// false to make them non-transactional, even if the database configuration says otherwise
        /// </summary>
        public bool Transactional{get; set;} = true;

        /// <summary>
        /// The jobs that make up the group
        /// </summary>
        public List<Job> Jobs{get;} = new();

        // <summary>
        /// True if the group and its jobs should be verbose with their logging
        /// </summary>
        public bool Verbose{get; init;}

        public async ValueTask<(bool Succeeded, Scorecard Scorecard)> Run()
        {
            var succeeded = false;
            var scorecard = new Scorecard(this);
            scorecard.StartUtc = DateTime.UtcNow;

            var log = MakeLog();
            log.Info("Running");

            try
            {
                Score? score = null;
                var context = this.Script.MakeContext(this);

                try
                {
                    foreach(var job in this.Jobs)
                    {
                        score = MakeScore(job, scorecard);

                        try
                        {
                            log.Info($"Running {job.Name}");

                            var dependencies = new RuntimeDependencies(context, score);                        
                            job.RegisterRuntimeDependencies(dependencies);

                            await job.Run().ContinueOnAnyContext();
                        }
                        catch(Exception e)
                        {
                            job.Log.Warn("Job failed", e);
                            throw;
                        }
                        finally
                        {
                            score.StopUtc = DateTime.UtcNow;

                            job.UnregisterRuntimeDependencies();
                        }
                    }

                    context.Commit();
                    succeeded = true;
                }
                catch(Exception e)
                {
                    // Something went wrong, but that might be allowed
                    if(this.AllowFail)
                    {
                        log.Warn("Group failed, but this is allowed", e);
                        score?.ReportWarning($"Group failed, but this is allowed. Message = {e.Message}");
                    }
                    else
                    {
                        log.Error("Group failed", e);
                        score?.ReportError($"Group failed. Message = {e.Message}");
                    }

                    // Even if it's allowed we still want to rollback
                    context.Rollback();
                    succeeded = this.AllowFail;
                }
            }
            finally
            {
                scorecard.StopUtc = DateTime.UtcNow;
                var elapsedTime = scorecard.StopUtc - scorecard.StartUtc;

                log.Info($"Finished. Time taken = {elapsedTime}");
            }

            return (succeeded, scorecard);
        }

        public override string ToString()
        {
            return $"Name = {Name}, Enabled = {Enabled}";
        }

        private Score MakeScore(Job job, Scorecard scorecard)
        {
            var score = new Score(job);
            score.StartUtc = DateTime.UtcNow;
            scorecard.Add(score);

            return score;
        }

        private ILog MakeLog()
        {
            var log = LogManager.GetDefaultLog();
            return new PrefixLog(log, $"[{Name}]");
        }
    }
}
