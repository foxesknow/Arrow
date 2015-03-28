using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Memory.Pools
{
	/// <summary>
	/// A pool of fixed sized blocks
	/// </summary>
	public class FixedSizePool : MemoryPool
	{
		private readonly Stack<byte[]> m_Buffers=new Stack<byte[]>();

		private readonly int m_NumberOfBuffers;
		private readonly int m_BufferSize;
		private readonly PoolMode m_PoolMode;

		private readonly object m_SyncRoot=new object();

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="numberOfBuffers">The number of buffers in the pool</param>
		/// <param name="bufferSize">The size of each buffer</param>
		public FixedSizePool(int numberOfBuffers, int bufferSize) : this(numberOfBuffers,bufferSize,PoolMode.Default)
		{
		}

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="numberOfBuffers">The number of buffers in the pool</param>
		/// <param name="bufferSize">The size of each buffer</param>
		/// <param name="poolMode">How the pool should behave</param>
		public FixedSizePool(int numberOfBuffers, int bufferSize, PoolMode poolMode)
		{
			if(numberOfBuffers<1) throw new ArgumentException("must specify at least 1 buffer","numberOfBuffers");
			if(bufferSize<1) throw new ArgumentException("buffer must be at least 1 byte","bufferSize");

			m_NumberOfBuffers=numberOfBuffers;
			m_BufferSize=bufferSize;
			m_PoolMode=poolMode;

			if(poolMode.IsSet(PoolMode.PreAllocate))
			{
				for(int i=0; i<numberOfBuffers; i++)
				{
					byte[] buffer=new byte[bufferSize];
					m_Buffers.Push(buffer);
				}
			}
		}

		/// <summary>
		/// The size of the buffers that will be pooled
		/// </summary>
		public int BufferSize
		{
			get{return m_BufferSize;}
		}

		/// <summary>
		/// Returns the number of buffers that the pool will manage
		/// </summary>
		public int NumberOfBuffers
		{
			get{return m_NumberOfBuffers;}
		}

		/// <summary>
		/// Returns the number of buffers available
		/// </summary>
		public int AvailableBuffers
		{
			get
			{
				lock(m_SyncRoot)
				{
					return m_Buffers.Count;
				}
			}
		}

		/// <summary>
		/// Checks out at least the number of bytes requested
		/// </summary>
		/// <param name="numberOfBytes">The number of bytes required</param>
		/// <returns>An array of at least the requested number of bytes</returns>
		public override byte[] Checkout(int numberOfBytes)
		{
			// If someone asks for too big a buffer then create one but
			// don't cache it when it's checked back in
			if(numberOfBytes>m_BufferSize) 
			{
				return new byte[numberOfBytes];
			}

			byte[] buffer=null;
			bool usedExistingBuffer=false;
			
			lock(m_SyncRoot)
			{
				if(m_Buffers.Count!=0)
				{
					buffer=m_Buffers.Pop();
					usedExistingBuffer=true;
				}
				else
				{
					buffer=new byte[m_BufferSize];
				}
			}

			if(usedExistingBuffer && m_PoolMode.IsSet(PoolMode.ClearOnCheckout)) 
			{
				Array.Clear(buffer,0,buffer.Length);
			}

			return buffer;
		}

		/// <summary>
		/// Returns a previously checked out buffer to the pool
		/// </summary>
		/// <param name="buffer">The buffer to check back in</param>
		public override void Checkin(byte[] buffer)
		{
			if(buffer==null) throw new ArgumentNullException("buffer");
			
			if(buffer.Length!=m_BufferSize)
			{
				// A request for too big a buffer is allowed, so
				// ignore any buffers that don't match our buffer size
				return; // NOTE: Early return
			}

			lock(m_SyncRoot)
			{
				if(m_Buffers.Count!=m_NumberOfBuffers)
				{
					m_Buffers.Push(buffer);
				}
			}
		}
	}
}
