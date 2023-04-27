using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.JobRunner
{
    /// <summary>
    /// Base class for JobRunner exceptions
    /// </summary>
    [Serializable]
    public class JobRunnerException : Exception
    {
        public JobRunnerException()
        { 
        }
        
        public JobRunnerException(string message) : base(message) 
        { 
        }
        
        public JobRunnerException(string message, Exception inner) : base(message, inner) 
        { 
        }
        
        protected JobRunnerException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) 
        { 
        }
    }
}
