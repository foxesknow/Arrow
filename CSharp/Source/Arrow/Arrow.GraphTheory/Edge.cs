using System;
using System.Collections.Generic;
using System.Text;

namespace Arrow.GraphTheory
{
	/// <summary>
	/// Defines an edge between 2 verticies (U,V)
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Edge<T> : IEquatable<Edge<T>>
	{
		private T m_From;
		private T m_To;
	
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="from">The first vertex</param>
		/// <param name="to">The second vertex</param>
		public Edge(T from, T to)
		{
			if(from==null) throw new ArgumentNullException("from");
			if(to==null) throw new ArgumentNullException("to");
			
			m_From=from;
			m_To=to;
		}
		
		/// <summary>
		/// Returns the first vertex
		/// </summary>
		public T From
		{
			get{return m_From;}
		}
		
		/// <summary>
		/// Returns the second vertex
		/// </summary>
		public T To
		{
			get{return m_To;}
		}

		/// <summary>
		/// Tests for equality
		/// </summary>
		/// <param name="obj">The right side to compare against</param>
		/// <returns>true if the objects are equal, otherwise false</returns>
		public override bool Equals(object obj)
		{
			Edge<T> rhs=obj as Edge<T>;
			if(rhs==null) return false;
			
			return Equals(rhs);
		}

		/// <summary>
		/// Generates a hashcode for the edge
		/// </summary>
		/// <returns>A hashcode</returns>
		public override int GetHashCode()
		{
			return m_From.GetHashCode()^m_To.GetHashCode();
		}

		/// <summary>
		/// Returns a string representation of the edge
		/// </summary>
		/// <returns>A string</returns>
		public override string ToString()
		{
			return string.Format("[{0}]->[{1}]",m_From,m_To);
		}

		#region IEquatable<Edge<T>> Members

		/// <summary>
		/// Tests for equality
		/// </summary>
		/// <param name="other">The right side to compare against</param>
		/// <returns>true if the objects are equal, otherwise false</returns>
		public bool Equals(Edge<T> other)
		{
			if(other==null) return false;
			
			return m_From.Equals(other.From) && m_To.Equals(other.To);
		}

		#endregion
	}
}
