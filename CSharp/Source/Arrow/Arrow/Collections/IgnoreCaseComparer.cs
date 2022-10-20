using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Collections
{
	/// <summary>
	/// Provides an implementation of a case insensitive string comparer
	/// </summary>
	public class IgnoreCaseComparer : IComparer<string>
	{
		/// <summary>
		/// A default instance
		/// </summary>
		public static readonly IComparer<string> Instance=new IgnoreCaseComparer();

		/// <summary>
		/// Compares two string
		/// </summary>
		/// <param name="x">The left side</param>
		/// <param name="y">The right side</param>
		/// <returns>The outcome of the comparison</returns>
		public int Compare(string? x, string? y)
		{
			return string.Compare(x,y,true);
		}
	}
}
