using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.AlertableData
{
    /// <summary>
    /// A class that can be used when there's no state data to be passed.
    /// </summary>
    public sealed class NoState
    {
        /// <summary>
        /// The only instance of the data
        /// </summary>
        public static readonly NoState Data = new();

        private NoState()
        {
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "no state data";
        }
    }
}
