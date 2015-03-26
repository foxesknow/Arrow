using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Memory.Pools
{
	/// <summary>
	/// Defines pool modes
	/// </summary>
	[Flags]
	public enum PoolMode
	{
		/// <summary>
		/// Default behaviour for the pool
		/// </summary>
		Default=0,
		
		/// <summary>
		/// Preallocate the pool buffers rather than allocate on demand
		/// </summary>
		PreAllocate=1,

		/// <summary>
		/// Zero out any data in a buffer before returning it
		/// </summary>
		ClearOnCheckout=2
	}

	static class PoolModeExtensions
	{
		public static bool IsSet(this PoolMode poolMode, PoolMode flag)
		{
			return (poolMode & flag)!=PoolMode.Default;
		}
	}
}
