using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Execution
{
    /// <summary>
    /// Represents a key whose underlying implementation isn't important.
    /// A new instance of the class is guaranteed to be unique
    /// </summary>
    [Serializable]
    public sealed class OpaqueKey : IEquatable<OpaqueKey>, IComparable<OpaqueKey>
    {
        private readonly Guid m_Key=Guid.NewGuid();

        /// <summary>
        /// Renders the key as a string
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return m_Key.ToString();
        }

        /// <summary>
        /// Generates a hash code for the key
        /// </summary>
        /// <returns>A hash code</returns>
        public override int GetHashCode()
        {
            return m_Key.GetHashCode();
        }

        /// <summary>
        /// Compares the object for equality
        /// </summary>
        /// <param name="obj">The object to compare against for equality</param>
        /// <returns>true if the objects are equal, otherwise false</returns>
        public override bool Equals(object? obj)
        {
            return Equals(obj as OpaqueKey);
        }

        /// <summary>
        /// Compares the object for equality
        /// </summary>
        /// <param name="other">The object to compare against for equality</param>
        /// <returns>true if the objects are equal, otherwise false</returns>
        public bool Equals(OpaqueKey? other)
        {
            if(other==null) return false;

            return m_Key==other.m_Key;
        }

        /// <summary>
        /// Compares the object for ordering
        /// </summary>
        /// <param name="other">The object to compare against</param>
        /// <returns>0 if equal, a negative number if this instance if less than the other, oterwise a positive number</returns>
        public int CompareTo(OpaqueKey ?other)
        {
            if(other==null) return -1;

            return m_Key.CompareTo(other.m_Key);
        }
    }
}
