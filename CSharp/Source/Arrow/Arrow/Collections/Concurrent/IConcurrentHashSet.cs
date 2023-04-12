using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Collections.Concurrent
{
    public interface IConcurrentHashSet<T> : IReadOnlyConcurrentHashSet<T> where T : notnull
    {
        /// <summary>
        /// Attempts to add a value to the hashset
        /// </summary>
        /// <param name="value"></param>
        /// <returns>true if the item was added, false if not</returns>
        public bool Add(T value);

        /// <summary>
        /// Tries to remove the vale
        /// </summary>
        /// <param name="value"></param>
        /// <returns>true if removed, false if the value is not present</returns>
        public bool Remove(T value);

        /// <summary>
        /// Removes all data from the hashset
        /// </summary>
        public void Clear();
    }
}
