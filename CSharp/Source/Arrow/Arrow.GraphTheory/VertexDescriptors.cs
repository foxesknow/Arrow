using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Arrow.GraphTheory
{
	/// <summary>
	/// Maps a vertex to a vertex descriptor
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class VertexDescriptors<T> : KeyedCollection<T,VertexDescriptor<T>>
	{
		internal VertexDescriptors(IEqualityComparer<T> comparer) : base(comparer)
		{
		}
	
		/// <summary>
		/// Returns the key for a descriptor
		/// </summary>
		/// <param name="item">The descriptor</param>
		/// <returns>The vertex within the descriptor</returns>
		protected override T GetKeyForItem(VertexDescriptor<T> item)
		{
			return item.Vertex;
		}
	}
}
