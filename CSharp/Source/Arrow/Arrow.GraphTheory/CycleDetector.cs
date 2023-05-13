using System;
using System.Collections.Generic;
using System.Text;

namespace Arrow.GraphTheory
{
	/// <summary>
	/// Detects cycles in a graph
	/// </summary>
	/// <typeparam name="T">The type of the vertex</typeparam>
	public class CycleDetector<T> : DepthFirstSearchVisitor<T>
	{
        private bool m_HasCycle = false;

        private Edge<T> m_CycleEdge;

        /// <summary>
        /// Called when a back edge is detected.
        /// Since this means we're going back up the DFS tree there must be a cycle
        /// </summary>
        /// <param name="edge">The edge</param>
        public override void BackEdge(Edge<T> edge)
        {
            m_HasCycle = true;
            m_CycleEdge = edge;
        }

        /// <summary>
        /// True if a cycle was detected, otherwise false
        /// </summary>
        public bool HasCycle
		{
			get{return m_HasCycle;}
		}
		
		/// <summary>
		/// Returns the edge that caused the cycle
		/// </summary>
		public Edge<T> Edge
		{
			get{return m_CycleEdge;}
		}
	}
}
