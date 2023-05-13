using System;
using System.Collections.Generic;
using System.Text;

namespace Arrow.GraphTheory
{
	/// <summary>
	/// Defines an graph where the edges have no direction.
	/// This meand that for an edge (U,V) U->V and V->U
	/// </summary>
	/// <typeparam name="T">The type of the vertices in the list</typeparam>
	public class UndirectedGraph<T> : Graph<T>
	{
        /// <summary>
        /// Initializes the instance
        /// </summary>
        public UndirectedGraph() : base()
        {
        }

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="equalityComparer">The comparer to use. May be null</param>
        public UndirectedGraph(IEqualityComparer<T> equalityComparer) : base(equalityComparer)
        {

        }

        /// <summary>
        /// Adds an edge to the list
        /// </summary>
        /// <param name="edge">The edge to add</param>
        public override void Add(Edge<T> edge)
        {
            if(edge == null) throw new ArgumentNullException("edge");
            if(edge.From.Equals(edge.To)) throw new ArgumentException("self referencing edge");

            // Add the edge U->V
            DoAdd(edge);

            // Add the reverse edge to get a edge coming back
            DoAdd(new Edge<T>(edge.To, edge.From));
        }

        /// <summary>
        /// Calculates the in-degree for each vertex.
        /// VertexDescriptor.D holds the degee
        /// </summary>
        /// <returns></returns>
        public VertexDescriptors<T> CalculateDegree()
        {
            VertexDescriptors<T> descriptors = CreateVertexDescriptors();

            // First, create a descriptor for each vertex
            foreach(T vertex in Vertices())
            {
                VertexDescriptor<T> descriptor = new VertexDescriptor<T>(vertex);
                descriptor.D = 0;
                descriptors.Add(descriptor);
            }

            // Now visit each edge
            foreach(Edge<T> edge in AllEdges())
            {
                VertexDescriptor<T> descriptor = descriptors[edge.To];
                descriptor.D++;
            }

            return descriptors;
        }
    }
}
