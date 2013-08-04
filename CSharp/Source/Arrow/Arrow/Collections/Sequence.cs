using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Collections
{
	/// <summary>
	/// Useful enumeration methods
	/// </summary>
	public static class Sequence
	{
		/// <summary>
		/// Generates a sequence that returns the first item and then the rest of the items
		/// </summary>
		/// <typeparam name="T">The type of item to enumerate over</typeparam>
		/// <param name="first">The first item to return</param>
		/// <param name="rest">The other items to return</param>
		/// <returns>An enumerator to all the items</returns>
		public static IEnumerable<T> Cons<T>(T first, IEnumerable<T> rest)
		{
			if(rest==null) throw new ArgumentNullException("rest");

			yield return first;

			foreach(var item in rest)
			{
				yield return item;
			}
		}

		/// <summary>
		/// Yields a single value
		/// </summary>
		/// <typeparam name="T">The type of data to yield</typeparam>
		/// <param name="value">The single value to yield</param>
		/// <returns>An enumeration</returns>
		public static IEnumerable<T> Single<T>(T value)
		{
			yield return value;
		}

		/// <summary>
		/// Returns an enumeration that has an extra value on the end
		/// </summary>
		/// <typeparam name="T">The type of data to yield</typeparam>
		/// <param name="sequence">The sequence to enumerate over</param>
		/// <param name="last">The value to return after the sequence has been enumerated</param>
		/// <returns>An enumeration</returns>
		public static IEnumerable<T> Append<T>(IEnumerable<T> sequence, T last)
		{
			if(sequence==null) throw new ArgumentNullException("sequence");

			foreach(T value in sequence)
			{
				yield return value;
			}

			yield return last;
		}
	}
}
