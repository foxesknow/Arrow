using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Collections
{
	/// <summary>
	/// Useful slice extensions
	/// </summary>
	public static class SliceExtensions
	{
		/// <summary>
		/// Creates a slice from a list
		/// </summary>
		/// <typeparam name="T">The type held in the list</typeparam>
		/// <param name="list">The list to create a slice from</param>
		/// <param name="start">Where the slice starts</param>
		/// <param name="count">How many items in the slice</param>
		/// <returns>A slice into the list</returns>
		public static Slice<T> AsSlice<T>(this IList<T> list, int start, int count)
		{
			return new Slice<T>(list,start,count);
		}
	}
}
