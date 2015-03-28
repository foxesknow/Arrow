using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.Memory.Pools
{
	/// <summary>
	/// A stepped pool contains a series of buffers that increase in size.
	/// Requests for memory are rounded to the nearest step size, so you may
	/// get back a bigger array that requested.
	/// 
	/// This class is thread safe.
	/// </summary>
	public class SteppedPool : MemoryPool
	{
		private const int Kilobyte=1024;
		
		private static readonly byte[] ZeroBuffer=new byte[0];
		
		private readonly byte[][] m_Steps;
		
		private readonly int m_NumberOfSteps;
		private readonly int m_StepSize;

		private readonly PoolMode m_PoolMode;

		/// <summary>
		/// Initializes the instance using 1K per step
		/// </summary>
		/// <param name="numberOfSteps">The number of steps</param>
		public SteppedPool(int numberOfSteps) : this(numberOfSteps,1,PoolMode.Default)
		{
		}

		/// <summary>
		/// Initializes the instance using 1K per step
		/// </summary>
		/// <param name="numberOfSteps">The number of steps</param>
		/// <param name="kilobytesPerStep">The number of kilobytes per step</param>
		public SteppedPool(int numberOfSteps, int kilobytesPerStep) : this(numberOfSteps,kilobytesPerStep,PoolMode.Default)
		{
		}

		/// <summary>
		/// Initializes the instance using 1K per step
		/// </summary>
		/// <param name="numberOfSteps">The number of steps</param>
		/// <param name="poolMode">How the pool should behave</param>
		public SteppedPool(int numberOfSteps, PoolMode poolMode) : this(numberOfSteps,1,poolMode)
		{
		}		

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="numberOfSteps">The number of steps</param>
		/// <param name="kilobytesPerStep">The number of kilobytes per step</param>
		/// <param name="poolMode">How the pool should behave</param>
		public SteppedPool(int numberOfSteps, int kilobytesPerStep, PoolMode poolMode)
		{
			if(numberOfSteps<1) throw new ArgumentException("must have at least one step","numberOfSteps");
			if(kilobytesPerStep<1) throw new ArgumentException("must have at least 1K per step","kilobytesPerStep");

			// Add 1 to numbers of steps as the first item in the index is for the empty buffer
			m_NumberOfSteps=numberOfSteps+1;
			m_StepSize=kilobytesPerStep*Kilobyte;
			m_PoolMode=poolMode;
			
			m_Steps=new byte[m_NumberOfSteps][];

			if(poolMode.IsSet(PoolMode.PreAllocate))
			{
				AllocateBuffers();
			}
		}

		/// <summary>
		/// Returns the number of steps in the pool
		/// </summary>
		public int NumberOfSteps
		{
			get{return m_NumberOfSteps-1;}
		}

		/// <summary>
		/// Returns the step size.
		/// Each step is Step*StepSize kilobytes big
		/// </summary>
		public int StepSize
		{
			get{return m_StepSize;}
		}

		/// <summary>
		/// Returns the number of steps that are available (not checked out) within the pool.
		/// Note that when this class is shared amongst multiple threads the return value
		/// should be treated as an approximation
		/// </summary>
		/// <returns>The number of steps that are available</returns>
		public int GetAvailableSteps()
		{
			int available=0;

			for(int i=1; i<m_NumberOfSteps; i++)
			{
				var buffer=Interlocked.CompareExchange(ref m_Steps[i],null,null);
				if(buffer!=null) available++;
			}

			return available;
		}

		/// <summary>
		/// Checks out at least the number of bytes requested
		/// </summary>
		/// <param name="numberOfBytes">The number of bytes required</param>
		/// <returns>An array of at least the requested number of bytes</returns>
		public override byte[] Checkout(int numberOfBytes)
		{
			if(numberOfBytes==0) return ZeroBuffer;

			byte[] buffer=null;
			
			// The buffers increase in size, so once we know where to start
			// we can look at it and all subsequent slots
			int allocationSize=CalculateAllocationSize(numberOfBytes);
			int slot=GetSlot(allocationSize);
			for(int i=slot; i<m_NumberOfSteps; i++)
			{
				// Try to swap out a buffer, if one already exists
				buffer=Interlocked.Exchange(ref m_Steps[i],null);
				
				if(buffer!=null) 
				{
					if(m_PoolMode.IsSet(PoolMode.ClearOnCheckout)) Array.Clear(buffer,0,buffer.Length);

					break;
				}
			}

			// If we couldn't find any existing buffers then we'll create a new one
			if(buffer==null)
			{
				buffer=new byte[allocationSize];
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

			// Make sure the buffer has the right size to be checked back in
			if((buffer.Length%(m_StepSize))!=0) throw new ArgumentException("invalid buffer size","buffer");

			int length=buffer.Length;
			if(length!=0)
			{
				// Swap the buffer back in.
				// If there's already one there then we'll discard the supplied one
				int slot=GetSlot(length);

				if(slot<m_NumberOfSteps)
				{
					Interlocked.CompareExchange(ref m_Steps[slot],buffer,null);
				}
			}
		}
		
		private int CalculateAllocationSize(int numberOfBytes)
		{
			return RoundToNearestBlockSize(numberOfBytes,m_StepSize);
		}

		private int GetSlot(int allocationSize)
		{
			int slot=allocationSize/m_StepSize;
			return slot;
		}

		private void AllocateBuffers()
		{
			for(int i=1; i<m_NumberOfSteps; i++)
			{
				int stepSize=i*m_StepSize;
				byte[] buffer=new byte[stepSize];

				m_Steps[i]=buffer;
			}
		}
	}
}
