using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace Arrow.Memory
{
	/// <summary>
	/// Manages a collection of array segments
	/// </summary>
	/// <typeparam name="T">The type of the segments</typeparam>
	public class ArraySegmentCollection<T> : IEnumerable<ArraySegment<T>>
	{
		private readonly List<ArraySegment<T>> m_Segments;

		/// <summary>
		/// Initializes the instance
		/// </summary>
		public ArraySegmentCollection()
		{
			m_Segments=new List<ArraySegment<T>>(8);
		}

		/// <summary>
		/// Initializes the instance
		/// <param name="capacity">The number of items the collection can initially store</param>
		/// </summary>
		public ArraySegmentCollection(int capacity)
		{
			if(capacity<0) throw new ArgumentOutOfRangeException("capacity","capacity must be at least zero");

			m_Segments=new List<ArraySegment<T>>(capacity);
		}

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="segments">The segments to initialize the collection with</param>
		public ArraySegmentCollection(IEnumerable<ArraySegment<T>> segments)
		{
			if(segments==null) throw new ArgumentNullException("segments");

			m_Segments=new List<ArraySegment<T>>(segments);
		}

		/// <summary>
		/// Adds a segment to the front of the collection (index 0)
		/// </summary>
		/// <param name="segment">The segment to add</param>
		public void AddFront(ArraySegment<T> segment)
		{
			m_Segments.Insert(0,segment);
		}

		/// <summary>
		/// Returns the sum of the length of all segments
		/// </summary>
		/// <returns>The overall number of items in the collection</returns>
		public int GetOverallLength()
		{
			int length=0;

			for(int i=0; i<m_Segments.Count; i++)
			{
				length+=m_Segments[i].Count;
			}

			return length;
		}

		/// <summary>
		/// Adds a segment to the end of the collection
		/// </summary>
		/// <param name="segment">The segment to add</param>
		public void AddBack(ArraySegment<T> segment)
		{
			m_Segments.Add(segment);
		}

		/// <summary>
		/// Removes all items from the collection
		/// </summary>
		public void Clear()
		{
			m_Segments.Clear();
		}

		/// <summary>
		/// Flattens all the segments into one single array
		/// </summary>
		/// <returns>An array containing all the items in the collection</returns>
		public T[] ToArray()
		{
			int totalLength=GetOverallLength();
			var data=new T[totalLength];
			int offset=0;

			for(int i=0; i<m_Segments.Count; i++)
			{
				var segment=m_Segments[i];
				Array.Copy(segment.Array,segment.Offset,data,offset,segment.Count);
				offset+=segment.Count;
			}

			return data;
		}

		/// <summary>
		/// Copies all the segments into another a destination segment.
		/// The destination segment must have space for all the data in the collection
		/// </summary>
		/// <param name="destinationSegment">The destination segment</param>
		public void CopyIntoSegment(ArraySegment<T> destinationSegment)
		{
			int totalLength=GetOverallLength();
			if(destinationSegment.Count<totalLength) throw new ArgumentException("destinationSegment","not enough space in destination");

			var data=destinationSegment.Array;
			int offset=destinationSegment.Offset;

			for(int i=0; i<m_Segments.Count; i++)
			{
				var segment=m_Segments[i];
				Array.Copy(segment.Array,segment.Offset,data,offset,segment.Count);
				offset+=segment.Count;
			}
		}

		/// <summary>
		/// Enumerates over all the items in the individual segment
		/// </summary>
		/// <returns>An enumerable</returns>
		public IEnumerable<T> Items()
		{
			for(int i=0; i<m_Segments.Count; i++)
			{
				var segment=m_Segments[i];

				var array=segment.Array;
				int offset=segment.Offset;
				int end=offset+segment.Count;

				for(int index=offset; index<end; index++)
				{
					yield return array[index];
				}
			}
		}

		/// <summary>
		/// Returns the number of segments in the collection
		/// </summary>
		public int Count
		{
			get{return m_Segments.Count;}
		}

		/// <summary>
		/// Returns the segment at the specified index
		/// </summary>
		/// <param name="index">The index to fetch</param>
		/// <returns>The segment as the specified index</returns>
		public ArraySegment<T> this[int index]
		{
			get{return m_Segments[index];}
		}

		/// <summary>
		/// Returns the underlying segments in the collection.
		/// </summary>
		public IList<ArraySegment<T>> UnderlyingSegments
		{
			get{return m_Segments;}
		}

		/// <summary>
		/// Returns an enumerator that yields each segment in the collection
		/// </summary>
		/// <returns>An enumerator</returns>
		public IEnumerator<ArraySegment<T>> GetEnumerator()
		{
			return m_Segments.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
