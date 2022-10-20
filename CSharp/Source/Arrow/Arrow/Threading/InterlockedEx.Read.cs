using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Arrow.Threading
{
	public static partial class InterlockedEx
	{
		/// <summary>
		/// Reads a value from a location
		/// </summary>
		/// <param name="location">The location to read</param>
		/// <returns>The value at the location</returns>
		public static long Read(ref long location)
		{
			return Interlocked.Read(ref location);
		}

		/// <summary>
		/// Reads a value from a location
		/// </summary>
		/// <param name="location">The location to read</param>
		/// <returns>The value at the location</returns>
		public static float Read(ref float location)
		{
			return Interlocked.CompareExchange(ref location,0,0);
		}

		/// <summary>
		/// Reads a value from a location
		/// </summary>
		/// <param name="location">The location to read</param>
		/// <returns>The value at the location</returns>
		public static double Read(ref double location)
		{
			return Interlocked.CompareExchange(ref location,0,0);
		}

		/// <summary>
		/// Reads a value from a location
		/// </summary>
		/// <param name="location">The location to read</param>
		/// <returns>The value at the location</returns>
		public static IntPtr Read(ref IntPtr location)
		{
			return Interlocked.CompareExchange(ref location,IntPtr.Zero,IntPtr.Zero);
		}

		/// <summary>
		/// Reads a value from a location
		/// </summary>
		/// <param name="location">The location to read</param>
		/// <returns>The value at the location</returns>
		public static object? Read(ref object? location)
		{
			return Interlocked.CompareExchange(ref location,null,null);
		}

		/// <summary>
		/// Reads a value from a location
		/// </summary>
		/// <param name="location">The location to read</param>
		/// <returns>The value at the location</returns>
		public static T? Read<T>(ref T? location) where T:class
		{
			return Interlocked.CompareExchange(ref location,null,null);
		}
	}
}
