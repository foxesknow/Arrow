using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Memory.Pools
{
	/// <summary>
	/// A memory pool that always allocates new memory
	/// </summary>
	public class AlwaysAllocatePool : MemoryPool
	{
		/// <summary>
		/// Checks out at least the number of bytes requested
		/// </summary>
		/// <param name="numberOfBytes">The number of bytes required</param>
		/// <returns>An array of at least the requested number of bytes</returns>
		public override byte[] Checkout(int numberOfBytes)
		{
			return new byte[numberOfBytes];
		}

		/// <summary>
		/// Does nothing to do the buffer
		/// </summary>
		/// <param name="buffer"></param>
		public override void Checkin(byte[] buffer)
		{
			// No need to do anything
		}
	}
}
