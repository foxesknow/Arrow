using System;
using System.Collections.Generic;
using System.Text;

namespace Arrow.GraphTheory
{
	/// <summary>
	/// Defines a visitor for the shortest path algorithm
	/// </summary>
	/// <typeparam name="T">The type of the vertex</typeparam>
	public abstract class ShortestPathVisitor<T>
	{
		/// <summary>
		/// Returns a value to represent infinity
		/// </summary>
		public abstract double InfiniteValue{get;}
		
		/// <summary>
		/// Determines the weight of an edge
		/// </summary>
		/// <param name="edge">The edge to examine</param>
		/// <returns>The weight of the edge</returns>
		public abstract double WeightOf(Edge<T> edge);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <param name="edge"></param>
		/// <returns></returns>
		public abstract bool IsCloser(VertexDescriptor<T> lhs, VertexDescriptor<T> rhs, Edge<T> edge);
		
		/// <summary>
		/// Returns 0
		/// </summary>
		public virtual double StartWeight
		{
			get{return 0;}
		}
		
		/// <summary>
		/// Adds two weights together
		/// </summary>
		/// <param name="lhs">The left wight</param>
		/// <param name="rhs">The right weight</param>
		/// <returns>The total of the weights</returns>
		public double Add(double lhs, double rhs)
		{
			return lhs+rhs;
		}
		
	}
}
