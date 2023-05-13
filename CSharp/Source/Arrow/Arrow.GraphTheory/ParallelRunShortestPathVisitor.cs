using System;
using System.Collections.Generic;
using System.Text;

namespace Arrow.GraphTheory
{
	/// <summary>
	/// Works out what items can be build at the same time in
	/// a makefile type scenario. VertexDescriptors with the 
	/// same D value can be build at the same time
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ParallelRunShortestPathVisitor<T> : ShortestPathVisitor<T>
	{
		/// <summary>
		/// Returns a very small negative number
		/// </summary>
		public override double InfiniteValue
		{
			get{return -999999;}
		}

		/// <summary>
		/// Always returns 1
		/// </summary>
		/// <param name="edge">The edge to weigh</param>
		/// <returns>1</returns>
		public override double WeightOf(Edge<T> edge)
		{
			return 1;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <param name="edge"></param>
		/// <returns></returns>
		public override bool IsCloser(VertexDescriptor<T> lhs, VertexDescriptor<T> rhs, Edge<T> edge)
		{
            return lhs.D + WeightOf(edge) > rhs.D;
        }
    }
}
