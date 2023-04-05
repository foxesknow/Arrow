using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow
{
    /// <summary>
    /// A unit type, similiar to that found in most functional languages
    /// </summary>
    public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>
    {
        /// <summary>
        /// A default instance
        /// </summary>
        public static readonly Unit Default = new();

        /// <inheritdoc/>
        public int CompareTo(Unit other)
        {
            return 0;
        }

        /// <inheritdoc/>
        public bool Equals(Unit other)
        {
            return true;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return 0;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "()";
        }

        /// <inheritdoc/>
        public override bool Equals(object? other)
        {
            return other is Unit;
        }

        /// <summary>
        /// Tests for equality. Always returns true
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator ==(Unit lhs, Unit rhs)
        {
            return true;
        }

        /// <summary>
        /// Tests for inequality. Always returns false
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator !=(Unit lhs, Unit rhs)
        {
            return false;
        }
    }
}
