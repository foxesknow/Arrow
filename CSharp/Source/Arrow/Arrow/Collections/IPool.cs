using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Arrow.Collections
{
	/// <summary>
	/// Defines a pool of objects that are stored in a pool
	/// to avoid GC hits
	/// </summary>
	/// <typeparam name="T">The type of item to pool</typeparam>
	public interface IPool<T>
	{
		/// <summary>
		/// Returns an item to the pool
		/// </summary>
		/// <param name="item">The item to return</param>
		void Checkin(T item);
		
		/// <summary>
		/// Removes an item from the pool, or creates a new item if there's no space in the pool
		/// </summary>
		/// <returns>An item from the pool</returns>
		T Checkout();
		
		/// <summary>
		/// Checks out a pool item is one is immediately available
		/// </summary>
		/// <param name="item">Set to a pooled item if available, otherwise the default for T</param>
		/// <returns>true if an item was checked out, false if not</returns>
		bool TryCheckout([MaybeNullWhen(false)]out T item);

		/// <summary>
		/// Returns the number of items available in the pool
		/// </summary>
		int Count{get;}
	}
}
