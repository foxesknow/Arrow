using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Arrow.Collections
{
	/// <summary>
	/// Default implementation of a pool, which has no upperbound on the number of items it may pool
	/// This implementation is thread safe
	/// </summary>
	/// <typeparam name="T">The type of data to store in the pool</typeparam>
	public class Pool<T> : IPool<T>
	{
		private readonly object m_SyncRoot=new object();
		
		private Stack<T> m_Items=new Stack<T>();
		
		private Func<T> m_ItemFactory;
		
		/// <summary>
		/// Initializes the instance with an initial size of zero
		/// </summary>
		/// <param name="factory">A function responsible for creating the items in the pool</param>
		public Pool(Func<T> factory) : this(0,factory)
		{
		}
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="initialSize">The initial number of items to have in the pool</param>
		/// <param name="factory">A function responsible for creating the items in the pool</param>
		public Pool(int initialSize, Func<T> factory)
		{
			if(initialSize<0) throw new ArgumentException("initialSize<0");
			if(factory==null) throw new ArgumentNullException("factory");
			
			m_ItemFactory=factory;
			
			for(int i=0; i<initialSize; i++)
			{
				T item=m_ItemFactory();
				m_Items.Push(item);
			}
		}
	
		/// <summary>
		/// Returns an item to the pool
		/// </summary>
		/// <param name="item">The item to return</param>
		public void Checkin(T item)
		{
			lock(m_SyncRoot)
			{
				m_Items.Push(item);
			}
		}

		/// <summary>
		/// Removes an item from the pool
		/// </summary>
		/// <returns>An item from the pool</returns>
		public T Checkout()
		{
			lock(m_SyncRoot)
			{
				if(m_Items.Count!=0) 
				{
					return m_Items.Pop();
				}
				else
				{
					// As we're empty we'll create a new item
					return m_ItemFactory();
				}
			}
		}
		
		/// <summary>
		/// Checks out a pool item is one is immediately available
		/// </summary>
		/// <param name="item">Set to a pooled item if available, otherwise the default for T</param>
		/// <returns>true if an item was checked out, false if not</returns>
		public bool TryCheckout([MaybeNullWhen(false)] out T item)
		{
			lock(m_SyncRoot)
			{
				if(m_Items.Count!=0)
				{
					item=m_Items.Pop();
					return true;
				}
				else
				{
					item=default!;
					return false;
				}
			}
		}

		/// <summary>
		/// Returns the number of items available in the pool
		/// </summary>
		public int Count
		{
			get
			{
				lock(m_SyncRoot)
				{
					return m_Items.Count;
				}
			}
		}
	}
}
