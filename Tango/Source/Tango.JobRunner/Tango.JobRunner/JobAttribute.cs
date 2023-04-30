using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Workbench
{
    /// <summary>
    /// Flags a class as a job
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class JobAttribute : Attribute
    {
        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="name">The name the job will be known by</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public JobAttribute(string name)
        {
            if(name is null) throw new ArgumentNullException(nameof(name));
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name is empty", nameof(name));

            Name = name;
        }

        public string Name{get;}
    }
}
