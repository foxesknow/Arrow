using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Collections.Concurrent
{
    public interface IReadOnlyConcurrentHashSet<T> : IEnumerable<T> where T : notnull
    {
        /// <summary>
        /// Checks to see if the item exists
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(T value);

        /// <summary>
        /// True if there is data in the hashset, false otherwise
        /// </summary>
        /// <returns></returns>
        public bool HasData();

        /// <summary>
        /// True is the hashset is empty, false otherwise
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty();
    }
}
