using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Memory
{
	/// <summary>
	/// Useful ArraySegmentCollection factory methods
	/// </summary>
	public static class ArraySegmentCollection
	{
		/// <summary>
		/// Creates an ArraySegmentCollection from an existing array
		/// </summary>
		/// <typeparam name="T">The type of collectio</typeparam>
		/// <param name="data">The data to create a segment form</param>
		/// <returns>A collection</returns>
		public static ArraySegmentCollection<T> FromArray<T>(T[] data)
		{
			if(data==null) throw new ArgumentNullException("data");

			var segment=new ArraySegment<T>(data);
			return FromSegment(segment);			
		}

		/// <summary>
		/// Creates an ArraySegmentCollection from a memory stream.
		/// No copy is made of the data in the memory stream. 
		/// A segment is added that refers directly to the underlying buffer with
		/// a count equal to the current position of the stream
		/// </summary>
		/// <param name="memoryStream">The memory stream to use</param>
		/// <returns>A collection</returns>
		public static ArraySegmentCollection<byte> FromMemoryStream(MemoryStream memoryStream)
		{
			if(memoryStream==null) throw new ArgumentNullException("memoryStream");

			var buffer=memoryStream.GetBuffer();
			int length=(int)memoryStream.Position;

			var segment=new ArraySegment<byte>(buffer,0,length);
			return FromSegment(segment);
		}

		/// <summary>
		/// Creates a collection from a single segment
		/// </summary>
		/// <typeparam name="T">The type of the data</typeparam>
		/// <param name="segment">The single segment to initialize the collection with</param>
		/// <returns>A collection</returns>
		public static ArraySegmentCollection<T> FromSegment<T>(ArraySegment<T> segment)
		{
			var collection=new ArraySegmentCollection<T>();
			collection.AddBack(segment);

			return collection;
		}
	}
}
