using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading;

namespace Arrow.Threading.Collections
{
	/// <summary>
	/// This class provides compare and swap (CAS) versions of all the mutatable methods
	/// in the generic List class. It does this my creating a copy of the source list,
	/// applying the mutatable operation and then swapping the new list back in.
	/// 
	/// Because of the copy and swap semantics of this class it is best used in situations
	/// where the source list is not particulary large or where the list will be
	/// written to occassionaly but read many more times.
	/// </summary>
	public class SwapList<T> : IList<T>
	{	
		private List<T> m_List;

		/// <summary>
		/// Initializes the instance with an empty list
		/// </summary>
		public SwapList()
		{
			m_List=new List<T>();
		}

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="collection">A sequence of values to add</param>
		public SwapList(IEnumerable<T> collection)
		{
			m_List=new List<T>(collection);
		}

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="capacity">The initial capacity of the list</param>
		public SwapList(int capacity)
		{
			m_List=new List<T>(capacity);
		}

		/// <summary>
		/// Returns the list as a readonly collection
		/// </summary>
		/// <returns>A readonly collection</returns>
		public ReadOnlyCollection<T> AsReadOnly()
		{
			var list=Interlocked.CompareExchange(ref m_List,null,null);
			return list.AsReadOnly();
		}

		/// <summary>
		/// Searches the list for a particular item
		/// </summary>
		/// <param name="item">The item to look for</param>
		/// <returns>The zero-based index of the item, or -1 if not found</returns>
		public int IndexOf(T item)
		{
			var list=Interlocked.CompareExchange(ref m_List,null,null);
			return list.IndexOf(item);
		}

		/// <summary>
		/// Searches the list for a particular item, starting at the specified index
		/// </summary>
		/// <param name="item">The item to look for</param>
		/// <param name="index">The index to start searching from</param>
		/// <returns>The zero-based index of the item, or -1 if not found</returns>
		public int IndexOf(T item, int index)
		{
			var list=Interlocked.CompareExchange(ref m_List,null,null);
			return list.IndexOf(item,index);
		}

		/// <summary>
		/// Searches the list for a particular item in a given range
		/// </summary>
		/// <param name="item">The item to look for</param>
		/// <param name="index">The index to start searching from</param>
		/// <param name="count">How many items to examine, from the start of index</param>
		/// <returns>The zero-based index of the item, or -1 if not found</returns>
		public int IndexOf(T item, int index, int count)
		{
			var list=Interlocked.CompareExchange(ref m_List,null,null);
			return list.IndexOf(item,index,count);
		}

		/// <summary>
		/// Inserts an item at a particular index
		/// </summary>
		/// <param name="index">The index to add the item to</param>
		/// <param name="item">The item to add</param>
		public void Insert(int index, T item)
		{
			List<T> current;
			List<T> existing=m_List;

			do
			{
				current=existing;
				var newList=Copy(current);
				newList.Insert(index,item);

				existing=Interlocked.CompareExchange(ref m_List,newList,current);
			}while(current!=existing);
		}		

		/// <summary>
		/// Gets or sets the item at the specified index
		/// </summary>
		/// <param name="index">The index of the item to get or set</param>
		/// <returns>The item at the specified index</returns>
		public T this[int index]
		{
			get
			{
				var list=Interlocked.CompareExchange(ref m_List,null,null);
				return list[index];
			}
			set
			{
				List<T> current;
				List<T> existing=m_List;

				do
				{
					current=existing;
					var newList=Copy(current);
					newList[index]=value;

					existing=Interlocked.CompareExchange(ref m_List,newList,current);
				}while(current!=existing);
			}
		}

		/// <summary>
		/// Adds an item to the list
		/// </summary>
		/// <param name="item">The list to add to</param>
		public void Add(T item)
		{
			List<T> current;
			List<T> existing=m_List;

			do
			{
				current=existing;
				var newList=Copy(current);
				newList.Add(item);

				existing=Interlocked.CompareExchange(ref m_List,newList,current);
			}while(current!=existing);
		}

		/// <summary>
		/// Adds a range of values to the list
		/// </summary>
		/// <param name="collection">The values to add</param>
		public void AddRange(IEnumerable<T> collection)
		{
			List<T> current;
			List<T> existing=m_List;

			do
			{
				current=existing;
				var newList=Copy(current);
				newList.AddRange(collection);
				
				existing=Interlocked.CompareExchange(ref m_List,newList,current);
			}while(current!=existing);
		}

		/// <summary>
		/// Removes all items from the list
		/// </summary>
		public void Clear()
		{
			List<T> current;
			List<T> existing=m_List;

			do
			{
				current=existing;
				var newList=Copy(current);
				newList.Clear();
				
				existing=Interlocked.CompareExchange(ref m_List,newList,current);
			}while(current!=existing);
		}

		/// <summary>
		/// Determines if an item is in the list
		/// </summary>
		/// <param name="item">The item to check for</param>
		/// <returns>true if the item is in the list, false otherwise</returns>
		public bool Contains(T item)
		{
			var list=Interlocked.CompareExchange(ref m_List,null,null);
			return list.Contains(item);
		}

		/// <summary>
		/// Copies the entire list to a compatible 1D array, starting at the specified index in the target array
		/// </summary>
		/// <param name="array">The array to write to</param>
		/// <param name="arrayIndex">The index at which copying should begin</param>
		public void CopyTo(T[] array, int arrayIndex)
		{
			var list=Interlocked.CompareExchange(ref m_List,null,null);
			list.CopyTo(array,arrayIndex);
		}

		/// <summary>
		/// Returns a new array with the contents of the list in it
		/// </summary>
		/// <returns>An array</returns>
		public T[] ToArray()
		{
			var list=Interlocked.CompareExchange(ref m_List,null,null);
			return list.ToArray();
		}

		/// <summary>
		/// Returns the number of items in the list
		/// </summary>
		public int Count
		{
			get
			{
				var list=Interlocked.CompareExchange(ref m_List,null,null);
				return list.Count;
			}
		}

		/// <summary>
		/// Always returns false
		/// </summary>
		public bool IsReadOnly
		{
			get{return false;}			
		}

		/// <summary>
		/// Removes a specific item from the list
		/// </summary>
		/// <param name="item">The item to remove</param>
		/// <returns>true if the item was removed, false otherwise</returns>
		public bool Remove(T item)
		{
			bool removed=false;

			List<T> current;
			List<T> existing=m_List;

			do
			{
				current=existing;
				var newList=Copy(current);
				removed=newList.Remove(item);
				
				existing=Interlocked.CompareExchange(ref m_List,newList,current);
			}while(current!=existing);

			return removed;
		}

		/// <summary>
		/// Removes an item at the specified index
		/// </summary>
		/// <param name="index">The index of the item to remove</param>
		public void RemoveAt(int index)
		{
			List<T> current;
			List<T> existing=m_List;

			do
			{
				current=existing;
				var newList=Copy(current);
				newList.RemoveAt(index);
				
				existing=Interlocked.CompareExchange(ref m_List,newList,current);
			}while(current!=existing);
		}

		/// <summary>
		/// Removes all items that match a condition
		/// </summary>
		/// <param name="match">The condition that determines if an item should be removed</param>
		/// <returns>The number of items removed</returns>
		public int RemoveAll(Predicate<T> match)
		{
			int numberRemoved=0;

			List<T> current;
			List<T> existing=m_List;

			do
			{
				current=existing;
				var newList=Copy(current);
				numberRemoved=newList.RemoveAll(match);
				
				existing=Interlocked.CompareExchange(ref m_List,newList,current);
			}while(current!=existing);

			return numberRemoved;
		}

		/// <summary>
		/// Removes a range of values
		/// </summary>
		/// <param name="index">The zero-based starting index of the range of elements to remove</param>
		/// <param name="count">The number of elements to remove</param>
		public void RemoveRange(int index, int count)
		{
			List<T> current;
			List<T> existing=m_List;

			do
			{
				current=existing;
				var newList=Copy(current);
				newList.RemoveRange(index,count);
				
				existing=Interlocked.CompareExchange(ref m_List,newList,current);
			}while(current!=existing);
		}

		/// <summary>
		/// Determines if any item in the list satisfies a condition
		/// </summary>
		/// <param name="match">The condition to apply</param>
		/// <returns>true if an item matches the condition, false otherwise</returns>
		public bool Exists(Predicate<T> match)
		{
			var list=Interlocked.CompareExchange(ref m_List,null,null);
			return list.Exists(match);
		}

		/// <summary>
		/// Searches a sorted list from an item
		/// </summary>
		/// <param name="item">The item to search for</param>
		/// <returns>The zero-based index of item in the sorted list,
		/// if item is found; otherwise, a negative number that is the bitwise complement
		/// of the index of the next element that is larger than item or, if there is
		/// no larger element, the bitwise complement of Count.</returns>
		public int BinarySearch(T item)
		{
			var list=Interlocked.CompareExchange(ref m_List,null,null);
			return list.BinarySearch(item);
		}

		/// <summary>
		/// Searches a sorted list for an item
		/// </summary>
		/// <param name="item">The item to search for</param>
		/// <param name="comparer">The comparer to use when comparing items</param>
		/// <returns>The zero-based index of item in the sorted list,
		/// if item is found; otherwise, a negative number that is the bitwise complement
		/// of the index of the next element that is larger than item or, if there is
		/// no larger element, the bitwise complement of Count.</returns>
		public int BinarySearch(T item, IComparer<T> comparer)
		{
			var list=Interlocked.CompareExchange(ref m_List,null,null);
			return list.BinarySearch(item,comparer);
		}

		/// <summary>
		/// Searches a sorted list for an item within a range
		/// </summary>
		/// <param name="index">The start index to start searching from</param>
		/// <param name="count">The number of items to examine</param>
		/// <param name="item">The item to search for</param>
		/// <param name="comparer">The comparer to use</param>
		/// <returns>The zero-based index of item in the sorted list,
		/// if item is found; otherwise, a negative number that is the bitwise complement
		/// of the index of the next element that is larger than item or, if there is
		/// no larger element, the bitwise complement of Count.</returns>
		public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
		{
			var list=Interlocked.CompareExchange(ref m_List,null,null);
			return list.BinarySearch(index,count,item,comparer);
		}

		/// <summary>
		/// Searches for an item that matches the specified condition and returns the first occurence
		/// </summary>
		/// <param name="match">The condition to apply</param>
		/// <returns>The first item that matches the condition, or the default value for type T if nothing is found</returns>
		public T Find(Predicate<T> match)
		{
			var list=Interlocked.CompareExchange(ref m_List,null,null);
			return list.Find(match);
		}

		/// <summary>
		/// Retrieves all items that match the specified condition
		/// </summary>
		/// <param name="match">The condition to apply</param>
		/// <returns>A list containing all matching items</returns>
		public List<T> FindAll(Predicate<T> match)
		{
			var list=Interlocked.CompareExchange(ref m_List,null,null);
			return list.FindAll(match);
		}

		/// <summary>
		/// Returns the index of the first item which matches a condition
		/// </summary>
		/// <param name="match">The condition to use</param>
		/// <returns>The zero-based index of the first item to match the condition, or -1 if nothing matches</returns>
		public int FindIndex(Predicate<T> match)
		{
			var list=Interlocked.CompareExchange(ref m_List,null,null);
			return list.FindIndex(match);
		}

		/// <summary>
		/// Returns the index of the first item which matches a condition, starting from a given index
		/// </summary>
		/// <param name="startIndex">Where to start searching from</param>
		/// <param name="match">The condition to use</param>
		/// <returns>The zero-based index of the first item to match the condition, or -1 if nothing matches</returns>
		public int FindIndex(int startIndex, Predicate<T> match)
		{
			var list=Interlocked.CompareExchange(ref m_List,null,null);
			return list.FindIndex(startIndex,match);
		}

		/// <summary>
		/// Returns the index of the first item which matches a condition within a given range
		/// </summary>
		/// <param name="startIndex">Where to start searching from</param>
		/// <param name="count">The number of items to examine</param>
		/// <param name="match">The condition to use</param>
		/// <returns>The zero-based index of the first item to match the condition, or -1 if nothing matches</returns>
		public int FindIndex(int startIndex, int count, Predicate<T> match)
		{
			var list=Interlocked.CompareExchange(ref m_List,null,null);
			return list.FindIndex(startIndex,count,match);
		}

		/// <summary>
		/// Searches for an item that matches the specified condition and returns the last occurence
		/// </summary>
		/// <param name="match">The condition to apply</param>
		/// <returns>The first item that matches the condition, or the default value for type T if nothing is found</returns>
		public T FindLast(Predicate<T> match)
		{
			var list=Interlocked.CompareExchange(ref m_List,null,null);
			return list.FindLast(match);
		}

		/// <summary>
		/// Searches for an item that matches the specified condition
		/// </summary>
		/// <param name="match">The condition to apply</param>
		/// <returns>The zero-based index of the last item to match the condition, or -1 if nothing matches</returns>
		public int FindLastIndex(Predicate<T> match)
		{
			var list=Interlocked.CompareExchange(ref m_List,null,null);
			return list.FindLastIndex(match);
		}

		/// <summary>
		/// Searches for an item that matches the specified condition
		/// </summary>
		/// <param name="startIndex">The index to start searching from</param>
		/// <param name="match">The condition to use</param>
		/// <returns>The zero-based index of the last item to match the condition, or -1 if nothing matches</returns>
		public int FindLastIndex(int startIndex, Predicate<T> match)
		{
			var list=Interlocked.CompareExchange(ref m_List,null,null);
			return list.FindLastIndex(startIndex,match);
		}

		/// <summary>
		/// Searches for an item that matches the specified condition
		/// </summary>
		/// <param name="startIndex">Where to start searching from</param>
		/// <param name="count">The number of items to examine</param>
		/// <param name="match">The condition to use</param>
		/// <returns>The zero-based index of the last item to match the condition, or -1 if nothing matches</returns>
		public int FindLastIndex(int startIndex, int count, Predicate<T> match)
		{
			var list=Interlocked.CompareExchange(ref m_List,null,null);
			return list.FindLastIndex(startIndex,count,match);
		}

		/// <summary>
		/// Converts the items in the list to a new type, and returns a list containing those items
		/// </summary>
		/// <typeparam name="TOutput">The type to convert to</typeparam>
		/// <param name="converter">The converter to apply</param>
		/// <returns>A new list containing the converted items</returns>
		public List<TOutput> ConvertAll<TOutput>(Converter<T,TOutput> converter)
		{
			var list=Interlocked.CompareExchange(ref m_List,null,null);
			return list.ConvertAll(converter);
		}

		/// <summary>
		/// Performs an action on each item in the list
		/// </summary>
		/// <param name="action">The action to perform</param>
		public void ForEach(Action<T> action)
		{
			var list=Interlocked.CompareExchange(ref m_List,null,null);
			list.ForEach(action);
		}

		/// <summary>
		/// Determines if a condition is true for all items in the list
		/// </summary>
		/// <param name="match">The condition to apply to the items</param>
		/// <returns>true if the condition is true for everything, otherwise false</returns>
		public bool TrueForAll(Predicate<T> match)
		{
			var list=Interlocked.CompareExchange(ref m_List,null,null);
			return list.TrueForAll(match);
		}

		/// <summary>
		/// Sorts the list
		/// </summary>
		public void Sort()
		{
			List<T> current;
			List<T> existing=m_List;

			do
			{
				current=existing;
				var newList=Copy(current);
				newList.Sort();
				
				existing=Interlocked.CompareExchange(ref m_List,newList,current);
			}while(current!=existing);
		}

		/// <summary>
		/// Sorts the list using the specified comparison
		/// </summary>
		/// <param name="comparison">The comparison to use</param>
		public  void Sort(Comparison<T> comparison)
		{
			List<T> current;
			List<T> existing=m_List;

			do
			{
				current=existing;
				var newList=Copy(current);
				newList.Sort(comparison);
				
				existing=Interlocked.CompareExchange(ref m_List,newList,current);
			}while(current!=existing);
		}

		/// <summary>
		/// Sorts the list using the specified comparer
		/// </summary>
		/// <param name="comparer">The comparer to use</param>
		public void Sort(IComparer<T> comparer)
		{
			List<T> current;
			List<T> existing=m_List;

			do
			{
				current=existing;
				var newList=Copy(current);
				newList.Sort(comparer);
				
				existing=Interlocked.CompareExchange(ref m_List,newList,current);
			}while(current!=existing);
		}

		/// <summary>
		/// Sorts a specifiec range of the list
		/// </summary>
		/// <param name="index">The zero-based starting index to sort</param>
		/// <param name="count">The number of items to sort</param>
		/// <param name="comparer">The comparer to use</param>
		public void Sort(int index, int count, IComparer<T> comparer)
		{
			List<T> current;
			List<T> existing=m_List;

			do
			{
				current=existing;
				var newList=Copy(current);
				newList.Sort(index,count,comparer);
				
				existing=Interlocked.CompareExchange(ref m_List,newList,current);
			}while(current!=existing);
		}

		/// <summary>
		/// Removes and excess space from the list
		/// </summary>
		public void TrimExcess()
		{
			List<T> current;
			List<T> existing=m_List;

			do
			{
				current=existing;
				var newList=Copy(current);
				newList.TrimExcess();
				
				existing=Interlocked.CompareExchange(ref m_List,newList,current);
			}while(current!=existing);
		}

		/// <summary>
		/// Reverses the contents of the list
		/// </summary>
		public void Reverse()
		{
			List<T> current;
			List<T> existing=m_List;

			do
			{
				current=existing;
				var newList=Copy(current);
				newList.Reverse();
				
				existing=Interlocked.CompareExchange(ref m_List,newList,current);
			}while(current!=existing);
		}

		/// <summary>
		/// Reverses the order of the elements in the specified range
		/// </summary>
		/// <param name="index">The zero-based starting index of the range to reverse</param>
		/// <param name="count">The number of elements in the range to reverse</param>
		public void Reverse(int index, int count)
		{
			List<T> current;
			List<T> existing=m_List;

			do
			{
				current=existing;
				var newList=Copy(current);
				newList.Reverse(index,count);
				
				existing=Interlocked.CompareExchange(ref m_List,newList,current);
			}while(current!=existing);
		}

		/// <summary>
		/// Returns an enumerator to values within the list.
		/// NOTE: It IS same to update the list after calling this method.
		/// However, any changes will not be visible
		/// </summary>
		/// <returns>An enumerator</returns>
		public IEnumerator<T> GetEnumerator()
		{
			var list=Interlocked.CompareExchange(ref m_List,null,null);
			return list.GetEnumerator();
		}


		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private List<T> Copy(List<T> list)
		{
			List<T> newList=new List<T>(list);
			return newList;
		}
	}
}
