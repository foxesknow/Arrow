using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.JobRunner
{
    public sealed class Score
    {
        private readonly List<string> m_Errors = new();
        private readonly List<string> m_Warnings = new();

        public Score(Job job)
        {
            if(job is null) throw new ArgumentNullException(nameof(job));

            this.Job = job;
        }

        public Job Job{get;}

        public DateTime StartUtc{get; internal set;}
        
        public DateTime StopUtc{get; internal set;}

        /// <summary>
        /// True if we had no erros or warnings, otherwise false
        /// </summary>
        public bool NoErrorsOrWarnings
        {
            get{return this.HasErrors && this.HasWarnings;}
        }

        /// <summary>
        /// True if there are errors, otherwise false
        /// </summary>
        public bool HasErrors
        {
            get{return m_Errors.Count != 0;}
        }

        /// <summary>
        /// True if there are warnings, otherwise false
        /// </summary>
        public bool HasWarnings
        {
            get{return m_Warnings.Count != 0;}
        }

        public IReadOnlyList<string> Errors
        {
            get{return m_Errors;}
        }

        public IReadOnlyList<string> Warnings
        {
            get{return m_Warnings;}
        }

        public void ReportError(string error)
        {
            m_Errors.Add(error);
        }

        public void ReportWarning(string warning)
        {
            m_Warnings.Add(warning);
        }
    }
}
