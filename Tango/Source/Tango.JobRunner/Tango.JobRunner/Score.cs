using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.JobRunner
{
    /// <summary>
    /// The score for a job.
    /// This class is thread safe
    /// </summary>
    public sealed class Score
    {
        private readonly List<string> m_Errors = new();
        private readonly List<string> m_Warnings = new();

        private readonly object m_SyncRoot = new();

        internal Score(Job job)
        {
            if(job is null) throw new ArgumentNullException(nameof(job));

            this.Job = job;
        }

        /// <summary>
        /// The job the score relates to
        /// </summary>
        public Job Job{get;}

        /// <summary>
        /// When the job was started
        /// </summary>
        public DateTime StartUtc{get; internal set;}
        
        /// <summary>
        /// When the job stopped
        /// </summary>
        public DateTime StopUtc{get; internal set;}

        /// <summary>
        /// True if we had no erros or warnings, otherwise false
        /// </summary>
        public bool NoErrorsOrWarnings
        {
            get
            {
                lock(m_SyncRoot)
                {
                    return this.HasErrors && this.HasWarnings;
                }
            }
        }

        /// <summary>
        /// True if there are errors, otherwise false
        /// </summary>
        public bool HasErrors
        {
            get
            {
                lock(m_SyncRoot)
                {
                    return m_Errors.Count != 0;
                }
            }
        }

        /// <summary>
        /// True if there are warnings, otherwise false
        /// </summary>
        public bool HasWarnings
        {
            get
            {
                lock(m_SyncRoot)
                {
                    return m_Warnings.Count != 0;
                }
            }
        }

        /// <summary>
        /// Returns a copy of the errors
        /// </summary>
        public IReadOnlyList<string> GetErrors()
        {
            lock(m_SyncRoot)
            {
                return m_Errors.ToArray();
            }
        }

        /// <summary>
        /// Returns a copy of the warnings
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<string> GetWarnings()
        {
            lock(m_SyncRoot)
            {
                return m_Warnings.ToArray();
            }
        }

        /// <summary>
        /// Adds an error
        /// </summary>
        /// <param name="error"></param>
        public void ReportError(string error)
        {
            lock(m_SyncRoot)
            {
                m_Errors.Add(error);
            }
        }

        /// <summary>
        /// Adds an exception. 
        /// If the exception is an aggregate then the the inner exceptions are added.
        /// </summary>
        /// <param name="exception"></param>
        public void ReportError(Exception exception)
        {
            lock(m_SyncRoot)
            {
                Recurse(exception);
            }

            void Recurse(Exception exception)
            {
                if(exception is AggregateException aggregate)
                {
                    foreach(var e in aggregate.InnerExceptions)
                    {
                        Recurse(e);
                    }
                }
                else
                {
                    m_Errors.Add(exception.Message);
                }
            }
        }

        /// <summary>
        /// Adds a warning
        /// </summary>
        /// <param name="warning"></param>
        public void ReportWarning(string warning)
        {
            lock(m_SyncRoot)
            {
                m_Warnings.Add(warning);
            }
        }
    }
}
