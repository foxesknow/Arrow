using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Logging;
using Arrow.Threading;
using Arrow.Threading.Tasks;

namespace Tango.JobRunner
{
    /// <summary>
    /// A group is a collection of jobs
    /// </summary>
    public sealed class Group
    {
        public Group(RunSheet script)
        {
            if(script is null) throw new ArgumentNullException(nameof(script));

            this.Script = script;
        }

        public RunSheet Script{get;}

        public string Name{get; init;} = "No name";

        public bool Enabled{get; init;} = true;

        public bool AllowFail{get; set;}

        public bool Transactional{get; set;} = true;

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
                        
                            job.Context = context;                            
                            await job.Run().ContinueOnAnyContext();
                        }
                        catch(Exception e)
                        {
                            if(this.AllowFail)
                            {
                                job.Log.Warn("Job failed, but this is allowed", e);
                                score.ReportWarning($"Job failed, but this is allowed. Message = {e.Message}");
                            }
                            else
                            {
                                job.Log.Error("Job failed", e);
                                score.ReportError($"Job failed. Message = {e.Message}");
                            }
                        }
                        finally
                        {
                            score.StopUtc = DateTime.UtcNow;

                            job.Context = null!;
                            job.Score = null!;
                        }
                    }

                    context.Commit();
                    succeeded = true;
                }
                catch(Exception e)
                {
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

                    context.Rollback();
                    succeeded = this.AllowFail;
                }
            }
            finally
            {
                scorecard.StopUtc = DateTime.UtcNow;
                var elapsedTime = scorecard.StartUtc - scorecard.StopUtc;

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

            job.Score = score;

            return score;
        }

        private ILog MakeLog()
        {
            var log = LogManager.GetDefaultLog();
            return new PrefixLog(log, $"[{Name}]");
        }
    }
}
