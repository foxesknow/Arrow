using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Collections
{
	/// <summary>
	/// Provides a zero based view into a list at a given index.
	/// The view is readonly for any operation that would alter the size of the outer list (such as Add or Clear).
	/// However, items can be replaces via the index operator as this does not affect the size of the underlying list.
	/// </summary>
	/// <typeparam name="T">The type of the slice</typeparam>
	public class Slice<T> : IList<T>
	{
		private readonly IList<T> m_List;
		private readonly int m_Start;
		private readonly int m_Count;

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="list">The list to slice into</param>
		/// <param name="start">Where the slice should start within the list</param>
		/// <param name="count">How many items in the slice</param>
		public Slice(IList<T> list, int start, int count)
		{
			if(list==null) throw new ArgumentNullException("list");			
			if(start<0) throw new ArgumentException("start");	
			if(start>=list.Count) throw new ArgumentException("start");
			if(count<0) throw new ArgumentException("count");
			if(start+count>list.Count) throw new ArgumentException("count");

			m_List=list;
			m_Start=start;
			m_Count=count;
		}

		#region IList<T> Members

		/// <summary>
		/// Returns the index of an item
		/// </summary>
		/// <param name="item">The item to find</param>
		/// <returns>The zero based index of the item, or -1 if not found</returns>
		public int IndexOf(T item)
		{
			var comparer=EqualityComparer<T>.Default;

			for(int i=0; i<m_Count; i++)
			{
				var value=m_List[m_Start+i];
				if(comparer.Equals(item,value)) return i;
			}

			return -1;
		}

		/// <summary>
		/// Not implemented
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		public void Insert(int index, T item)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Not implemented
		/// </summary>
		/// <param name="index"></param>
		public void RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Provides access to an item at a given index
		/// </summary>
		/// <param name="index">The item to access</param>
		/// <returns>The item</returns>
		public T this[int index]
		{
			get
			{
				if(index>=0 && index <m_Count)
				{
					return m_List[m_Start+index];
				}
				else
				{
					throw new IndexOutOfRangeException();
				}
			}
			set
			{
				if(index>=0 && index <m_Count)
				{
					m_List[m_Start+index]=value;
				}
				else
				{
					throw new IndexOutOfRangeException();
				}
			}
		}

		#endregion

		#region ICollection<T> Members

		/// <summary>
		/// Not implemented
		/// </summary>
		/// <param name="item"></param>
		public void Add(T item)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Not implemented
		/// </summary>
		public void Clear()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Determines if an item exists in the slice
		/// </summary>
		/// <param name="item">The item to find</param>
		/// <returns>true if the item exists, otherwise false</returns>
		public bool Contains(T item)
		{
			var comparer=EqualityComparer<T>.Default;

			for(int i=0; i<m_Count; i++)
			{
				var value=m_List[m_Start+i];
				if(comparer.Equals(item,value)) return true;
			}

			return false;
		}

		/// <summary>
		/// Copies the items in the slice to the array
		/// </summary>
		/// <param name="array">The array to copy to</param>
		/// <param name="arrayIndex">The start index in the array for copying</param>
		public void CopyTo(T[] array, int arrayIndex)
		{
			for(int i=0; i<m_Count; i++)
			{
				array[arrayIndex+i]=m_List[m_Start+i];
			}
		}

		/// <summary>
		/// Returns the number of items in the slice
		/// </summary>
		public int Count
		{
			get{return m_Count;}
		}

		/// <summary>
		/// Returns false
		/// </summary>
		public bool IsReadOnly
		{
			get{return false;}
		}

		/// <summary>
		/// Not implemented
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Remove(T item)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IEnumerable<T> Members

		/// <summary>
		/// An enumerator to the items in the slice
		/// </summary>
		/// <returns>An enumerator</returns>
		public IEnumerator<T> GetEnumerator()
		{
			for(int i=0; i<m_Count; i++)
			{
				yield return m_List[m_Start+i];
			}
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion
	}
}
