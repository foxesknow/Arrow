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
		/// Initializes the instance
		/// </summary>
		protected MemoryPool()
		{
		}

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

		/// <summary>
		/// Rounds a number up to the nearest multiple of a block size
		/// </summary>
		/// <param name="numberOfBytes">The number of bytes to round</param>
		/// <param name="blockSize">The block size to round to a multiple of</param>
		/// <returns>A block size that is a multiple of blockSize</returns>
		protected int RoundToNearestBlockSize(int numberOfBytes, int blockSize)
		{
			// Round to the nearest multipe of m_StepSize
			int blocks=(numberOfBytes+(blockSize-1))/blockSize;
			return blocks*blockSize;
		}
	}
}
