using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Memory.Pools
{
	/// <summary>
	/// Defines a memory pool
	/// </summary>
	public abstract class MemoryPool
	{
		/// <summary>
		/// Checks out at least the number of bytes requested
		/// </summary>
		/// <param name="numberOfBytes">The number of bytes required</param>
		/// <returns>An array of at least the requested number of bytes</returns>
		public abstract byte[] Checkout(int numberOfBytes);
		
		/// <summary>
		/// Returns a previously checked out buffer to the pool
		/// </summary>
		/// <param name="buffer">The buffer to check back in</param>
		public abstract void Checkin(byte[] buffer);
	}
}
