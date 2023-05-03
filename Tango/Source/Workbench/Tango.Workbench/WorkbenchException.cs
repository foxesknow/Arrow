using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Workbench
{
    /// <summary>
    /// Base class for Workbench exceptions
    /// </summary>
    [Serializable]
    public class WorkbenchException : Exception
    {
        public WorkbenchException()
        { 
        }
        
        public WorkbenchException(string? message) : base(message) 
        { 
        }
        
        public WorkbenchException(string? message, Exception inner) : base(message, inner) 
        { 
        }
        
        protected WorkbenchException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) 
        { 
        }
    }
}
