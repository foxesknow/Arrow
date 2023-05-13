using System;
using System.Collections.Generic;
using System.Text;

namespace Arrow.GraphTheory
{
	/// <summary>
	/// Holds information on a vertex
	/// </summary>
	/// <typeparam name="T">The type of the vertex</typeparam>
	public class VertexDescriptor<T>
	{
        private T m_Vertex;
        private T m_Parent = default(T);

        private VertexColor m_Color = VertexColor.White;

        private double m_D = -1;
        private double m_F = 0;

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="vertex">The vertex to describe</param>
        public VertexDescriptor(T vertex)
        {
            if(vertex == null) throw new ArgumentNullException("vertex");

            m_Vertex = vertex;
        }

        /// <summary>
        /// Returns the vertex
        /// </summary>
        public T Vertex
		{
			get{return m_Vertex;}
		}
		
		/// <summary>
		/// The parent of the vertex
		/// </summary>
		public T Parent
		{
			get{return m_Parent;}
			set{m_Parent = value;}
		}
		
		/// <summary>
		/// The color of the vertex
		/// </summary>
		public VertexColor Color
		{
			get{return m_Color;}
			set{m_Color = value;}
		}
		
		/// <summary>
		/// Holds the degree of a vertex
		/// </summary>
		public double D
		{
			get{return m_D;}
			set{m_D = value;}
		}
		
		/// <summary>
		/// TODO
		/// </summary>
		public double F
		{
			get{return m_F;}
			set{m_F = value;}
		}

        /// <summary>
        /// Extracts the vertices from a descriptor collection
        /// </summary>
        /// <param name="descriptors">The descriptors to process</param>
        /// <returns>A list of vertices</returns>
        public static List<T> Vertices(ICollection<VertexDescriptor<T>> descriptors)
        {
            if(descriptors == null) throw new ArgumentNullException("descriptors");

            List<T> vertices = new List<T>();

            foreach(VertexDescriptor<T> v in descriptors)
            {
                vertices.Add(v.Vertex);
            }

            return vertices;
        }
    }
}
