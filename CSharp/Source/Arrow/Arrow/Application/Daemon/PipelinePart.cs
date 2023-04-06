using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Application.Daemon
{
    /// <summary>
    /// Holds the information about each part of the pipeline.
    /// For example, "dir *.* | save" would be 2 pipeline parts
    /// </summary>
    public sealed class PipelinePart
    {
        public PipelinePart(string name, IReadOnlyList<string> arguments)
        {
            if(name is null) throw new ArgumentNullException(nameof(name));
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name is empty", nameof(name));
            if(arguments is null) throw new ArgumentNullException(nameof(arguments));

            this.Name = name;
            this.Arguments = arguments;
        }

        /// <summary>
        /// The name of the pipeline command
        /// </summary>
        public string Name{get;}

        /// <summary>
        /// And arguments to the pipeline command (empty if none supplied)
        /// </summary>
        public IReadOnlyList<string> Arguments{get;}

        /// <summary>
        /// Indicates if there are arguments for the command
        /// </summary>
        public bool HasArguments
        {
            get{return this.Arguments.Count != 0;}
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
