using System;
using System.Collections.Generic;
using System.Text;

namespace Arrow.GraphTheory.Build
{
	/// <summary>
	/// Groups together the targets that can be built in parallel
	/// </summary>
	/// <typeparam name="T">The type of the target</typeparam>
	public class ParallelBuildItems<T>
	{
		private List<T> m_Targets = new List<T>();
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		public ParallelBuildItems()
		{
		}
		
		/// <summary>
		/// Adds a target to the build
		/// </summary>
		/// <param name="target">The target to ask</param>
		public void Add(T target)
		{
			m_Targets.Add(target);
		}
		
		/// <summary>
		/// Returns all the targets that can be built in parallel
		/// </summary>
		public List<T> Targets
		{
			get{return m_Targets;}
		}
	}
}
