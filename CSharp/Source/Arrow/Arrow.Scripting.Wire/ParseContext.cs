using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;

namespace Arrow.Scripting.Wire
{
	public class ParseContext
	{
		private readonly HashSet<Assembly> m_References=new HashSet<Assembly>();
		private readonly List<string> m_Usings=new List<string>();

		/// <summary>
		/// The assemblies to reference when resolving types
		/// </summary>
		public HashSet<Assembly> References
		{
			get{return m_References;}
		}

		/// <summary>
		/// The namespaces to check when resolving types
		/// </summary>
		public IList<string> Usings
		{
			get{return m_Usings;}
		}
	}
}
