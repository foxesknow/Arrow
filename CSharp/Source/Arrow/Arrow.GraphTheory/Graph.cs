using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Arrow.GraphTheory
{
	/// <summary>
	/// Defines a grap
	/// </summary>
	/// <typeparam name="T">The type of the vertices in the graph</typeparam>
	public abstract class Graph<T>
	{
        private static readonly IList<Edge<T>> s_EmptyEdgeList = new ReadOnlyCollection<Edge<T>>(new List<Edge<T>>());

        // Maps a vertex to all the edges that use it
        private Dictionary<T, List<Edge<T>>> m_VertexMap;

        // All the edges in the list
        private List<Edge<T>> m_AllEdges = new List<Edge<T>>();

        // All the vertices in the list
        private HashSet<T> m_AllVertices;

        private IEqualityComparer<T> m_EqualityComparer;

        /// <summary>
        /// Initializes the instance
        /// </summary>
        protected Graph() : this(null)
		{
		}

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="equalityComparer">The comparer to use. May be null</param>
        protected Graph(IEqualityComparer<T> equalityComparer)
        {
            m_VertexMap = new Dictionary<T, List<Edge<T>>>(equalityComparer);
            m_AllVertices = new HashSet<T>(equalityComparer);

            m_EqualityComparer = equalityComparer;
            if(m_EqualityComparer == null) m_EqualityComparer = EqualityComparer<T>.Default;
        }

        /// <summary>
        /// Returns an object used to compare vertices
        /// </summary>
        protected IEqualityComparer<T> EqualityComparer
		{
			get{return m_EqualityComparer;}
		}
		
		/// <summary>
		/// Adds an edge to the list
		/// </summary>
		/// <param name="edge">The edge to add</param>
		public abstract void Add(Edge<T> edge);

        /// <summary>
        /// Adds a new edge to the graph
        /// </summary>
        /// <param name="from">The first vertex</param>
        /// <param name="to">The second vertex</param>
        public void Add(T from, T to)
        {
            Add(new Edge<T>(from, to));
        }

        /// <summary>
        /// Adds a standalone vertex to the list
        /// </summary>
        /// <param name="vertex">The vertex to add</param>
        public void AddVertex(T vertex)
        {
            if(vertex == null) throw new ArgumentNullException("vertex");
            m_AllVertices.Add(vertex);
        }

        /// <summary>
        /// Creates a new edge, but does not add it to the graph
        /// </summary>
        /// <param name="from">The first vertex</param>
        /// <param name="to">The second vertex</param>
        /// <returns>An edge</returns>
        public Edge<T> NewEdge(T from, T to)
        {
            return new Edge<T>(from, to);
        }

        /// <summary>
        /// Determines if an edge exists in the list
        /// </summary>
        /// <param name="edge">The edge to search for</param>
        /// <returns>true if the edge exists, false otherwise</returns>
        public bool ContainsEdge(Edge<T> edge)
        {
            if(edge == null) throw new ArgumentNullException("edge");

            bool contains = false;

            List<Edge<T>> edges = null;
            if(m_VertexMap.TryGetValue(edge.From, out edges))
            {
                contains = (FindEdge(edges, edge) != -1);
            }

            return contains;
        }

        /// <summary>
        /// Returns the number of edges
        /// </summary>
        public int EdgeCount
		{
			get{return m_AllEdges.Count;}
		}
		
		/// <summary>
		/// Returns the number of vertices
		/// </summary>
		public int VertexCount
		{
			get{return m_AllVertices.Count;}
		}

        /// <summary>
        /// Determines if a vertex exists in the list
        /// </summary>
        /// <param name="vertex">The vertex to search for</param>
        /// <returns>true if the vertex exists, false otherwise</returns>
        public bool ContainsVertex(T vertex)
        {
            if(vertex == null) throw new ArgumentNullException("vertex");

            return m_AllVertices.Contains(vertex);
        }

        /// <summary>
        /// Returns all the edges in the list
        /// </summary>
        /// <returns>A read-only list of all the edges</returns>
        public IList<Edge<T>> AllEdges()
        {
            return new ReadOnlyCollection<Edge<T>>(m_AllEdges);
        }

        /// <summary>
        /// Returns a list of all the vertices in the list
        /// </summary>
        /// <returns>A list of all the vertices</returns>
        public IList<T> Vertices()
        {
            return new List<T>(m_AllVertices);
        }

        /// <summary>
        /// Returns a list of edges connected to a vertex.
        /// In a directed graph this will be the edges (vertex,V)
        /// </summary>
        /// <param name="vertex">The vertex to check</param>
        /// <returns>A list of connected edges</returns>
        public IList<Edge<T>> EdgesConnectedTo(T vertex)
        {
            if(vertex == null) throw new ArgumentNullException("vertex");

            List<Edge<T>> vertices = null;
            if(m_VertexMap.TryGetValue(vertex, out vertices))
            {
                return new ReadOnlyCollection<Edge<T>>(vertices);
            }

            return s_EmptyEdgeList;
        }

        /// <summary>
        /// Create a VertexDescriptors instance for the graph
        /// </summary>
        /// <returns>A VertexDescriptors instance </returns>
        public VertexDescriptors<T> CreateVertexDescriptors()
        {
            return new VertexDescriptors<T>(this.EqualityComparer);
        }

        /// <summary>
        /// Performs a topological sort of the list
        /// </summary>
        /// <returns>Descriptors for the order of the visit</returns>
        public List<VertexDescriptor<T>> TopologicalSort()
        {
            List<VertexDescriptor<T>> list = new List<VertexDescriptor<T>>();
            DepthFirstSearch(new TopologicalVisitor<T>(list));

            return list;
        }

        /// <summary>
        /// Performs a breadth first search of the vertices, finding
        /// the shortest distance from a vertex to every reachable vertex.
        /// VertexDescriptor.D holds the distance
        /// </summary>
        /// <param name="startVertex">The vertex to start from</param>
        /// <returns>Descriptors for the order of the visit</returns>
        public VertexDescriptors<T> BreadthFirstSearch(T startVertex)
        {
            if(startVertex == null) throw new ArgumentNullException("startVertex");
            if(ContainsVertex(startVertex) == false) throw new ArgumentException("vertex is not in list");

            // First, create a descriptor for every vertex
            VertexDescriptors<T> descriptors = CreateVertexDescriptors();
            foreach(T vertex in Vertices())
            {
                descriptors.Add(new VertexDescriptor<T>(vertex));
            }

            // Flag the start vertex as being "discovered"
            VertexDescriptor<T> start = descriptors[startVertex];
            start.Color = VertexColor.Gray;
            start.D = 0;
            start.Parent = default(T);

            Queue<T> queue = new Queue<T>();
            queue.Enqueue(startVertex);

            while(queue.Count != 0)
            {
                T vertex = queue.Dequeue();
                foreach(Edge<T> edge in EdgesConnectedTo(vertex))
                {
                    VertexDescriptor<T> descriptor = descriptors[edge.To];
                    if(descriptor.Color == VertexColor.White)
                    {
                        descriptor.Color = VertexColor.Gray;
                        descriptor.D = descriptors[vertex].D + 1;
                        descriptor.Parent = vertex;
                        queue.Enqueue(edge.To);
                    }
                }

                // We've finished with vertex, so "blacken" it
                descriptors[vertex].Color = VertexColor.Black;
            }

            return descriptors;
        }

        /// <summary>
        /// Performs a depth first search
        /// </summary>
        /// <param name="visitor">The visitor to call whilst searching</param>
        /// <returns>Descriptors for the order of the visit</returns>
        public VertexDescriptors<T> DepthFirstSearch(DepthFirstSearchVisitor<T> visitor)
        {
            if(visitor == null) throw new ArgumentNullException("visitor");

            // First, create a descriptor for every vertex
            VertexDescriptors<T> descriptors = CreateVertexDescriptors();
            foreach(T vertex in Vertices())
            {
                descriptors.Add(new VertexDescriptor<T>(vertex));
            }

            int time = 0;
            foreach(T vertex in Vertices())
            {
                if(descriptors[vertex].Color == VertexColor.White)
                {
                    DepthFirstSearchVisit(descriptors, vertex, ref time, visitor);
                }
            }

            return descriptors;
        }

        private void DepthFirstSearchVisit(VertexDescriptors<T> descriptors, T u, ref int time, DepthFirstSearchVisitor<T> visitor)
        {
            time++;

            VertexDescriptor<T> uDescriptor = descriptors[u];
            visitor.StartVertex(uDescriptor);
            uDescriptor.Color = VertexColor.Gray;
            uDescriptor.D = time;

            // Explore all the edge (U,v)
            foreach(Edge<T> edge in EdgesConnectedTo(u))
            {
                VertexDescriptor<T> vDescriptor = descriptors[edge.To];

                if(vDescriptor.Color == VertexColor.White)
                {
                    visitor.TreeEdge(edge);
                    vDescriptor.Parent = u;
                    DepthFirstSearchVisit(descriptors, edge.To, ref time, visitor);
                }
                else if(vDescriptor.Color == VertexColor.Gray)
                {
                    visitor.BackEdge(edge);
                }
                else if(vDescriptor.Color == VertexColor.Black)
                {
                    visitor.ForwardOrCross(edge);
                }
            }

            // We're done with u, so "blacken" it
            uDescriptor.Color = VertexColor.Black;
            time++;
            uDescriptor.F = time;
            visitor.FinishVertex(uDescriptor);
        }

        /// <summary>
        /// Does the actual adding of an edge to the list
        /// </summary>
        /// <param name="edge">The edge to add</param>
        protected void DoAdd(Edge<T> edge)
        {
            List<Edge<T>> edges = GetEdgeList(edge.From);
            if(FindEdge(edges, edge) == -1)
            {
                edges.Add(edge);
                m_AllVertices.Add(edge.From);
                m_AllVertices.Add(edge.To);
                m_AllEdges.Add(edge);
            }

        }

        private List<Edge<T>> GetEdgeList(T vertex)
        {
            List<Edge<T>> edges = null;

            if(m_VertexMap.TryGetValue(vertex, out edges) == false)
            {
                edges = new List<Edge<T>>();
                m_VertexMap.Add(vertex, edges);
            }

            return edges;
        }

        private int FindEdge(List<Edge<T>> edges, Edge<T> edge)
        {
            return edges.FindIndex(delegate (Edge<T> e)
            {
                return m_EqualityComparer.Equals(edge.From, e.From) && m_EqualityComparer.Equals(edge.To, e.To);
            });
        }
    }
}
