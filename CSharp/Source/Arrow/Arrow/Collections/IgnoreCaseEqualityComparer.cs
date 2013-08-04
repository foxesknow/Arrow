using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Collections
{
	/// <summary>
	/// Provides an implementation of a case insensitive string equality comparer
	/// </summary>
	public class IgnoreCaseEqualityComparer : IEqualityComparer<string>
	{
		/// <summary>
		/// A default instance
		/// </summary>
		public static readonly IEqualityComparer<string> Instance=new IgnoreCaseEqualityComparer();

		/// <summary>
		/// Performs a case insensitive equality test
		/// </summary>
		/// <param name="x">The left side</param>
		/// <param name="y">The right side</param>
		/// <returns>true if equal, otherwise false</returns>
		public bool Equals(string x, string y)
		{
			return string.Compare(x,y,true)==0;
		}

		/// <summary>
		/// Returns a hash code
		/// </summary>
		/// <param name="obj">The string to generate a hash for</param>
		/// <returns>A hashcode for the string</returns>
		public int GetHashCode(string obj)
		{
			return obj.ToLower().GetHashCode();
		}
	}
}
