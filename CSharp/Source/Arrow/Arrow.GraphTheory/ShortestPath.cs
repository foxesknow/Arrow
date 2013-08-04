using System;
using System.Collections.Generic;
using System.Text;

namespace Arrow.GraphTheory
{
	/// <summary>
	/// Implements the shortest path algorithm
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ShortestPath<T>
	{
		private DirectedGraph<T> m_Graph;
		private ShortestPathVisitor<T> m_Visitor;
		
		private List<VertexDescriptor<T>> m_TopologicalSort;
		private VertexDescriptors<T> m_Descriptors;
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="graph">The graph to walk</param>
		/// <param name="visitor">The visitor to apply to vertices in the graph</param>
		public ShortestPath(DirectedGraph<T> graph, ShortestPathVisitor<T> visitor)
		{
			if(graph==null) throw new ArgumentNullException("graph");
			if(visitor==null) throw new ArgumentNullException("visitor");
			
			m_Graph=graph;
			m_Visitor=visitor;
			
			m_Descriptors=graph.CreateVertexDescriptors();
			
			Initialize();
		}
		
		private void Initialize()
		{
			foreach(T vertex in m_Graph.Vertices())
			{
				VertexDescriptor<T> descriptor=new VertexDescriptor<T>(vertex);
				descriptor.D=m_Visitor.InfiniteValue;
				m_Descriptors.Add(descriptor);
			}
			
			m_TopologicalSort=m_Graph.TopologicalSort();
		}
		
		/// <summary>
		/// Returns the distance
		/// </summary>
		public VertexDescriptors<T> Distances
		{
			get{return m_Descriptors;}
		}
		
		/// <summary>
		/// Calculates the shortest path for the vertex
		/// </summary>
		/// <param name="startVertex">The vertex to calculate the path for</param>
		public void Calculate(T startVertex)
		{
			if(startVertex==null) throw new ArgumentNullException("startVertex");
			
			m_Descriptors[startVertex].D=m_Visitor.StartWeight;
			
			foreach(VertexDescriptor<T> descriptor in m_TopologicalSort)
			{
				foreach(Edge<T> edge in m_Graph.EdgesConnectedTo(descriptor.Vertex))
				{
					Relax(edge);
				}
			}
		}
		
		private void Relax(Edge<T> edge)
		{
			VertexDescriptor<T> u=m_Descriptors[edge.From];
			VertexDescriptor<T> v=m_Descriptors[edge.To];
			
			if(m_Visitor.IsCloser(u,v,edge))
			{
				v.D=m_Visitor.Add(u.D,m_Visitor.WeightOf(edge));
				v.Parent=edge.From;
			}
		}
	}
}
