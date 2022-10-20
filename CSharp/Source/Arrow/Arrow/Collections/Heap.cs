using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Arrow.Collections
{
	/// <summary>
	/// A heap data structure. By default a maximum heap is created.
	/// The heap is non-stable and therefore does not guarantee to maintain
	/// the relative ordering of elements
	/// </summary>
	/// <typeparam name="T">The type of data to store</typeparam>
	[Serializable]
	public class Heap<T> : IEnumerable<T>
	{
		private readonly IComparer<T> m_Comparer;
		
		private readonly List<T> m_Data=new List<T>();
		
		/// <summary>
		/// Initializes the instance with a default comparer
		/// </summary>
		public Heap() : this((IComparer<T>?)null)
		{		
		}
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="comparer">The comparer to use. If null a default is used</param>
		public Heap(IComparer<T>? comparer)
		{
			if(comparer==null) comparer=Comparer<T>.Default;
			
			m_Comparer=comparer;
		}
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="data">Data to add to the head</param>
		public Heap(IEnumerable<T> data) : this(null,data)
		{
		}
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="comparer">The comparer to use. If null a default is used</param>
		/// <param name="data">The data to add to the heap</param>
		public Heap(IComparer<T>? comparer, IEnumerable<T> data) : this(comparer)
		{
			if(data==null) throw new ArgumentNullException("data");
		
			m_Data.AddRange(data);
			BuildHeap();
		}
		
		/// <summary>
		/// Removes all items from the heap
		/// </summary>
		public void Clear()
		{
			m_Data.Clear();
		}
		
		/// <summary>
		/// Adds a new item to the heap and rebalances the heap
		/// </summary>
		/// <param name="value">The value to add</param>
		public void Add(T value)
		{
			m_Data.Add(value);
			int index=m_Data.Count-1;
			
			IncreaseKey(index,value);
		}
		
		/// <summary>
		/// Adds a sequence of values to the heap
		/// </summary>
		/// <param name="range">The values to add</param>
		public void AddRange(IEnumerable<T> range)
		{
			foreach(T value in range)
			{
				Add(value);
			}
		}
		
		/// <summary>
		/// Returns the value at the specified index
		/// </summary>
		/// <param name="index">The index of the item to fetch</param>
		/// <returns>The value at the specified index</returns>
		public T this[int index]
		{
			get{return m_Data[index];}
		}
		
		/// <summary>
		/// Removes the top of the heap and rebalances the heap
		/// </summary>
		/// <returns>The top of the heap</returns>
		public T Extract()
		{
			int count=m_Data.Count;
			if(count==0) throw new InvalidOperationException("heap is empty");
			
			T max=m_Data[0];
			
			if(count==1)
			{
				m_Data.Clear();
			}
			else
			{
				int last=count-1;
				m_Data[0]=m_Data[last];
				m_Data.RemoveAt(last);
				Heapify(0);
			}
			
			return max;
		}
		
		/// <summary>
		/// Attempts to get the value to the left of an index
		/// </summary>
		/// <param name="index">The index to start from</param>
		/// <param name="value">On success the value to the left of index</param>
		/// <returns>true if a value was found, false otherwise</returns>
		public bool TryGetLeft(int index, [MaybeNullWhen(false)] out T value)
		{
			if(index<0 || index>=m_Data.Count) throw new IndexOutOfRangeException("index");
			
			int left=Left(index);
			
			if(left<m_Data.Count)
			{
				value=m_Data[left];
				return true;
			}
			else
			{
				value=default(T);
				return false;
			}
		}
		
		/// <summary>
		/// Attempts to get the value to the right of an index
		/// </summary>
		/// <param name="index">The index to start from</param>
		/// <param name="value">On success the value to the right of index</param>
		/// <returns>true if a value was found, false otherwise</returns>
		public bool TryGetRight(int index, [MaybeNullWhen(false)] out T value)
		{
			if(index<0 || index>=m_Data.Count) throw new IndexOutOfRangeException("index");
			
			int right=Right(index);
			
			if(right<m_Data.Count)
			{
				value=m_Data[right];
				return true;
			}
			else
			{
				value=default!;
				return false;
			}
		}
		
		private void Heapify(int index)
		{
			int l=Left(index);
			int r=Right(index);
			
			int largest=0;
			
			if(l<Count && m_Comparer.Compare(m_Data[l],m_Data[index])>0)
			{
				largest=l;
			}
			else
			{
				largest=index;
			}
			
			if(r<Count && m_Comparer.Compare(m_Data[r],m_Data[largest])>0)
			{
				largest=r;
			}
			
			if(largest!=index)
			{
				Exchange(index,largest);
				Heapify(largest);
			}
		}
		
		private void IncreaseKey(int index, T key)
		{
			if(m_Comparer.Compare(key,m_Data[index])<0) throw new InvalidOperationException();
			
			m_Data[index]=key;
			
			while(index>0 && m_Comparer.Compare(m_Data[Parent(index)],m_Data[index])<0)
			{
				Exchange(index,Parent(index));
				index=Parent(index);
			}
		}
		
		private bool HasIndex(int index)
		{
			return index<m_Data.Count;
		}
		
		private void BuildHeap()
		{
			int count=m_Data.Count;
			
			int start=(count/2)-1;
			if(start<0) return;
			
			for(int index=start; index>=0; index--)
			{
				Heapify(index);
			}
		}
		
		private void Exchange(int index1, int index2)
		{
			T temp=m_Data[index1];
			m_Data[index1]=m_Data[index2];
			m_Data[index2]=temp;
		}
		
		/// <summary>
		/// Returns the number of items in the heap
		/// </summary>
		public int Count
		{
			get{return m_Data.Count;}
		}
		
		private int Parent(int index)
		{
			return (index-1)/2;
		}
		
		/// <summary>
		/// Returns the index to the left of "index"
		/// </summary>
		/// <param name="index">The start index</param>
		/// <returns>The index to the left</returns>
		public int Left(int index)
		{
			return (index*2)+1;
		}
		
		/// <summary>
		/// Returns the index to the right of "index"
		/// </summary>
		/// <param name="index">The start index</param>
		/// <returns>The index to the right</returns>
		public int Right(int index)
		{
			return (index+1)*2;
		}

		/// <summary>
		/// Allows for enumberation over the heap
		/// </summary>
		/// <returns>An enumerator</returns>
		public IEnumerator<T> GetEnumerator()
		{
			return m_Data.GetEnumerator();
		}

		/// <summary>
		/// Allows for enumberation over the heap
		/// </summary>
		/// <returns>An enumerator</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
