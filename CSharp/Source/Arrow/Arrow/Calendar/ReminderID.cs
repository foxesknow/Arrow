using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Arrow.Calendar
{
    /// <summary>
    /// Uniquely identifies a reminder.
    /// The identifiers are comparable, with more recently allocated identifiers
    /// being "greater" than older identifiers
    /// </summary>
    public readonly struct ReminderID : IEquatable<ReminderID>, IComparable<ReminderID>
    {
        private static long s_NextID;

        /// <summary>
        /// No reminder id
        /// </summary>
        public static readonly ReminderID None = new();

        private readonly ulong m_ID;

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="id"></param>
        private ReminderID(ulong id)
        {
            m_ID = id;
        }

        /// <inheritdoc/>
        public bool Equals(ReminderID other)
        {
            return m_ID == other.m_ID;
        }

        /// <inheritdoc/>
        public int CompareTo(ReminderID other)
        {
            return m_ID.CompareTo(other.m_ID);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return m_ID.GetHashCode();
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is ReminderID other &&  m_ID == other.m_ID;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return m_ID.ToString();
        }

        /// <summary>
        /// Compares to identifiers for equality
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator ==(ReminderID lhs, ReminderID rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Compares to identifiers for inequality
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator !=(ReminderID lhs, ReminderID rhs)
        {
            return !lhs.Equals(rhs);
        }

        /// <summary>
        /// Allocates a new reminder identifier.
        /// The identifier is guaranteed to be unique 
        /// and will be "greater" than previous allocated identifiers.
        /// </summary>
        /// <returns></returns>
        public static ReminderID Allocate()
        {
            var id = unchecked((ulong)Interlocked.Increment(ref s_NextID));
            return new(id);
        }
    }
}
