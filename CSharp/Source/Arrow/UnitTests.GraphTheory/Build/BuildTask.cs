using System;
using System.Collections.Generic;
using System.Text;

using Arrow.GraphTheory.Build;

namespace UnitTests.Arrow.GraphTheory.Build
{
	class BuildTask : IBuildDescription<string>
	{
		private string m_Target;
		private List<string> m_Dependencies=new List<string>();
		
		public BuildTask(string target) : this(target,target)
		{
		}
		
		
		public BuildTask(string target, string firstDependency, params string[] rest)
		{
			m_Target=target;
			m_Dependencies.Add(firstDependency);
			m_Dependencies.AddRange(rest);
		}
		
		public void AddDependency(string dependency)
		{
			m_Dependencies.Add(dependency);
		}
	
		#region IBuildDescription<string> Members

		public ICollection<string> Dependencies
		{
			get{return m_Dependencies;}
		}

		public string Target
		{
			get{return m_Target;}
		}

		#endregion
	}
}
