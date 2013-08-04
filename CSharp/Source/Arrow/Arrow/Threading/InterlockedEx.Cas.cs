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
		/// Attempts to compare and swap a value
		/// </summary>
		/// <param name="target">The destination, whose value is compared with comparand and possibly replaced</param>
		/// <param name="value">The value that replaces the destination value if the comparison results in equality</param>
		/// <param name="comparand">The value that is compared to the value at target</param>
		/// <returns>true is target has been set to value, otherwise false</returns>
		public static bool Cas(ref int target, int value, int comparand)
		{
			return Interlocked.CompareExchange(ref target,value,comparand)==comparand;
		}

		/// <summary>
		/// Attempts to compare and swap a value
		/// </summary>
		/// <param name="target">The destination, whose value is compared with comparand and possibly replaced</param>
		/// <param name="value">The value that replaces the destination value if the comparison results in equality</param>
		/// <param name="comparand">The value that is compared to the value at target</param>
		/// <returns>true is target has been set to value, otherwise false</returns>
		public static bool Cas(ref float target, float value, float comparand)
		{
			return Interlocked.CompareExchange(ref target,value,comparand)==comparand;
		}

		/// <summary>
		/// Attempts to compare and swap a value
		/// </summary>
		/// <param name="target">The destination, whose value is compared with comparand and possibly replaced</param>
		/// <param name="value">The value that replaces the destination value if the comparison results in equality</param>
		/// <param name="comparand">The value that is compared to the value at target</param>
		/// <returns>true is target has been set to value, otherwise false</returns>
		public static bool Cas(ref double target, double value, double comparand)
		{
			return Interlocked.CompareExchange(ref target,value,comparand)==comparand;
		}

		/// <summary>
		/// Attempts to compare and swap a value
		/// </summary>
		/// <param name="target">The destination, whose value is compared with comparand and possibly replaced</param>
		/// <param name="value">The value that replaces the destination value if the comparison results in equality</param>
		/// <param name="comparand">The value that is compared to the value at target</param>
		/// <returns>true is target has been set to value, otherwise false</returns>
		public static bool Cas(ref IntPtr target, IntPtr value, IntPtr comparand)
		{
			return Interlocked.CompareExchange(ref target,value,comparand)==comparand;
		}

		/// <summary>
		/// Attempts to compare and swap a value
		/// </summary>
		/// <param name="target">The destination, whose value is compared with comparand and possibly replaced</param>
		/// <param name="value">The value that replaces the destination value if the comparison results in equality</param>
		/// <param name="comparand">The value that is compared to the value at target</param>
		/// <returns>true is target has been set to value, otherwise false</returns>
		public static bool Cas(ref object target, object value, object comparand)
		{
			return Interlocked.CompareExchange(ref target,value,comparand)==comparand;
		}

		/// <summary>
		/// Attempts to compare and swap a value
		/// </summary>
		/// <param name="target">The destination, whose value is compared with comparand and possibly replaced</param>
		/// <param name="value">The value that replaces the destination value if the comparison results in equality</param>
		/// <param name="comparand">The value that is compared to the value at target</param>
		/// <returns>true is target has been set to value, otherwise false</returns>
		public static bool Cas<T>(ref T target, T value, T comparand) where T:class
		{
			return Interlocked.CompareExchange(ref target,value,comparand)==comparand;
		}
	}
}
