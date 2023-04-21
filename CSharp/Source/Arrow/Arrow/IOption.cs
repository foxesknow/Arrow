using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow
{
    /// <summary>
    /// Defines the non-generic features of an option
    /// </summary>
    public interface IOption
    {
        /// <summary>
        /// True if the option has some value, otherwise false
        /// </summary>
        public bool IsSome{get;}

        /// <summary>
        /// True if the option is none, otherwise false
        /// </summary>
        public bool IsNone{get;}
    }
}
