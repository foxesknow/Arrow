using System;
using System.Collections.Generic;
using System.Text;

namespace Arrow.GraphTheory
{
	/// <summary>
	/// Defines the behaviour for a depth first search (DFS)
	/// </summary>
	/// <typeparam name="T">The type of the vertex</typeparam>
	public class DepthFirstSearchVisitor<T>
	{
        /// <summary>
        /// Initializes the instance
        /// </summary>
        public DepthFirstSearchVisitor()
        {
        }

        /// <summary>
        /// Called when a back edge is detected.
        /// A back edge is an edge that is up the DFS tree
        /// </summary>
        /// <param name="edge">The edge</param>
        public virtual void BackEdge(Edge<T> edge)
        {
        }

        /// <summary>
        /// Called when a tree edge is detected
        /// </summary>
        /// <param name="edge">The edge</param>
        public virtual void TreeEdge(Edge<T> edge)
        {
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="edge"></param>
        public virtual void ForwardOrCross(Edge<T> edge)
        {
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="item"></param>
        public virtual void StartVertex(VertexDescriptor<T> item)
        {
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="item"></param>
        public virtual void FinishVertex(VertexDescriptor<T> item)
        {
        }
    }
}
