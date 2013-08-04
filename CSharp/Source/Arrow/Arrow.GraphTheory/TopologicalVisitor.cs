using System;
using System.Collections.Generic;
using System.Text;

namespace Arrow.GraphTheory
{
	/// <summary>
	/// Defines a topological visitor.
	/// This is typically used to generate a topological sort which is a list
	/// of vertices such that each vertex comes before any vertex to which it has an edge
	/// </summary>
	/// <typeparam name="T">The type of the vertex</typeparam>
	public class TopologicalVisitor<T> : DepthFirstSearchVisitor<T>
	{
		private IList<VertexDescriptor<T>> m_Descriptors;
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="descriptors">A list that will be populated with the vertex descriptors</param>
		public TopologicalVisitor(IList<VertexDescriptor<T>> descriptors)
		{
			if(descriptors==null) throw new ArgumentNullException("descriptors");
			m_Descriptors=descriptors;
		}

		/// <summary>
		/// Called when the vertex is finished with
		/// </summary>
		/// <param name="item">The descriptor for the vertex</param>
		public override void FinishVertex(VertexDescriptor<T> item)
		{
			m_Descriptors.Insert(0,item);
		}
	}
}
