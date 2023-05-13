using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Functional
{
    /// <summary>
    /// Represents none
    /// </summary>
    public readonly struct None : IEquatable<None>, IEquatable<IOption>
    {
        /// <summary>
        /// Compares the instance against a None or an option
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if (obj is None) return true;
            if (obj is IOption other) return other.IsNone;

            return false;
        }

        /// <summary>
        /// Checks to see of the other side is None
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(None other)
        {
            return true;
        }

        /// <summary>
        /// Checks to see if the option is None
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IOption? other)
        {
            return other is IOption rhs && rhs.IsNone;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return 0;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "None";
        }
    }
}
