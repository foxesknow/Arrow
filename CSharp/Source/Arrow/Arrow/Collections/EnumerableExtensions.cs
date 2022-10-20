using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Collections
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Adds an item to the end of a sequence
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<T> AddToEnd<T>(this IEnumerable<T> sequence, T value)
        {
            if(sequence is null) throw new ArgumentNullException(nameof(sequence));
            return Execute(sequence, value);

            static IEnumerable<T> Execute(IEnumerable<T> sequence, T value)
            {
                foreach(var v in sequence)
                {
                    yield return v;
                }

                yield return value;
            }
        }
    }
}
