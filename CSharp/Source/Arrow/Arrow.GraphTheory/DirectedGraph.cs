using System;
using System.Collections.Generic;
using System.Text;

namespace Arrow.GraphTheory
{
	/// <summary>
	/// Defines an graph where there is direction between the vertices.
	/// For an edge (U,V) U is the source and V is the destination (U->V)
	/// </summary>
	public class DirectedGraph<T> : Graph<T>
	{
		/// <summary>
		/// Initializes the instance
		/// </summary>
		public DirectedGraph() : base()
		{
		}
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="equalityComparer">The comparer to use. May be null</param>
		public DirectedGraph(IEqualityComparer<T> equalityComparer) : base(equalityComparer)
		{
		
		}

        /// <summary>
        /// Adds an edge to the list
        /// </summary>
        /// <param name="edge">The edge to add</param>
        public override void Add(Edge<T> edge)
        {
            if(edge == null) throw new ArgumentNullException("edge");
            DoAdd(edge);
        }

        /// <summary>
        /// Calculates the in-degree for each vertex. 
        /// VertexDescriptor.D holds the in-degree, VertexDescriptor.F holds the out-degree
        /// </summary>
        /// <returns>Descriptors for every vertex in the graph</returns>
        public VertexDescriptors<T> CalculateInOutDegree()
        {
            VertexDescriptors<T> descriptors = CreateVertexDescriptors();

            // First, create a descriptor for each vertex...
            foreach(T vertex in Vertices())
            {
                VertexDescriptor<T> descriptor = new VertexDescriptor<T>(vertex);
                descriptor.D = 0;
                descriptor.F = 0;
                descriptors.Add(descriptor);
            }

            foreach(Edge<T> edge in AllEdges())
            {
                VertexDescriptor<T> from = descriptors[edge.From];
                from.F++;

                VertexDescriptor<T> to = descriptors[edge.To];
                to.D++;
            }

            return descriptors;
        }
    }
}
