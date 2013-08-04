using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Arrow.Threading
{
	/// <summary>
	/// An event that allows you to track outstanding work.
	/// It contains a handle which is set when nothing is outstanding and 
	/// a handle that is set when something is outstanding.
	/// </summary>
	public class OutstandingEvent : IDisposable
	{
		private readonly object m_Sync=new object();
		
		private readonly ManualResetEvent m_NothingOutstandingEvent=new ManualResetEvent(true);
		private readonly ManualResetEvent m_SomethingOutstandingEvent=new ManualResetEvent(false);
		
		private int m_Count;
		
		/// <summary>
		/// Increases the count by 1
		/// </summary>
		public void Increase()
		{
			Increase(1);
		}
		
		/// <summary>
		/// Increases the count
		/// </summary>
		/// <param name="amount">The amount to increase by (may be negative)</param>
		/// <exception cref="System.ArgumentException">If the amount would make the count less than zero</exception>
		public void Increase(int amount)
		{
			lock(m_Sync)
			{
				if(m_Count+amount<0) throw new ArgumentException("count would become less than 0");
				if(amount==0) return; // NOTE: Early return
				
				// We'll work out it we need to resignal the events, so save a call to kernel mode
				int previousCount=m_Count;
				m_Count+=amount;
				
				if(previousCount>0 && m_Count==0)
				{
					m_NothingOutstandingEvent.Set();
					m_SomethingOutstandingEvent.Reset();
				}
				else if(previousCount==0 && m_Count>0)
				{
					m_NothingOutstandingEvent.Reset();
					m_SomethingOutstandingEvent.Set();
				}
			}
		}
		
		/// <summary>
		/// Decreases the count by 1
		/// </summary>
		public void Decrease()
		{
			Increase(-1);
		}
		
		/// <summary>
		/// Decreases the count
		/// </summary>
		/// <param name="amount">The amount to decrease by</param>
		public void Decrease(int amount)
		{
			Increase(-amount);
		}
		
		/// <summary>
		/// Resets the count back to 0 and signals the wait handle
		/// </summary>
		public void Reset()
		{
			lock(m_Sync)
			{
				m_Count=0;
				m_NothingOutstandingEvent.Set();
				m_SomethingOutstandingEvent.Reset();
			}
		}
		
		/// <summary>
		/// Returns the current count
		/// </summary>
		public int Count
		{
			get
			{
				lock(m_Sync)
				{
					return m_Count;
				}
			}
		}
		
		/// <summary>
		/// A wait handle that is signalled when nothing is outstanding
		/// </summary>
		public WaitHandle NothingOutstandingHandle
		{
			get{return m_NothingOutstandingEvent;}
		}
		
		/// <summary>
		/// A wait handle that is signalled when something is outstanding
		/// </summary>
		public WaitHandle SomethingOutstandingHandle
		{
			get{return m_SomethingOutstandingEvent;}
		}
		
		/// <summary>
		/// Closes any handles used by the instance
		/// </summary>
		public void Close()
		{
			m_NothingOutstandingEvent.Close();
			m_SomethingOutstandingEvent.Close();
		}
	
		#region IDisposable Members

		/// <summary>
		/// Disposes of any resources
		/// </summary>
		public void Dispose()
		{
			Close();
		}

		#endregion
	}
}
