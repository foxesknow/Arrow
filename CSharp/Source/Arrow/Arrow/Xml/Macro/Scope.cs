using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Arrow.Xml.Macro
{
	/// <summary>
	/// Manages scoped data for the xml macro expander
	/// </summary>
	class Scope
	{
		private Dictionary<string,XmlNode> m_Macros=new Dictionary<string,XmlNode>();
		
		private Stack<string> m_IncludeStack;
	
		private Scope m_Parent;
		
		/// <summary>
		/// Initializes an instance
		/// </summary>
		/// <param name="parent">Any parent instance to defer to. This may be null</param>
		public Scope(Scope parent)
		{
			m_Parent=parent;
			
			if(m_Parent==null)
			{
				m_IncludeStack=new Stack<string>();
			}
			else
			{
				m_IncludeStack=m_Parent.m_IncludeStack;
			}
		}
		
		/// <summary>
		/// Adds a macro, replacing any with the same name
		/// </summary>
		/// <param name="name">The name of the macro</param>
		/// <param name="macro">The macro to add</param>
		public void Add(string name, XmlNode macro)
		{
			m_Macros[name]=macro;
		}
		
		/// <summary>
		/// Looks up a macro
		/// </summary>
		/// <param name="name">The name of the macro to fetch</param>
		/// <returns>The specified macro, or null if it does not exist</returns>
		public XmlNode GetMacro(string name)
		{
			XmlNode node=null;
			
			if(m_Macros.TryGetValue(name,out node)==false)
			{
				if(m_Parent!=null) node=m_Parent.GetMacro(name);
			}
			
			return node;
		}
		
		/// <summary>
		/// Stores an include file in the scope
		/// </summary>
		/// <param name="filename">The name of the file to store</param>
		public void PushInclude(string filename)
		{
			m_IncludeStack.Push(filename);
		}
		
		/// <summary>
		/// Returns the most recently stored include file
		/// </summary>
		/// <returns>A filename</returns>
		public string PopInclude()
		{
			return m_IncludeStack.Pop();
		}
		
		/// <summary>
		/// Returns the depth of the include stack
		/// </summary>
		/// <value>The include stack depth</value>
		public int IncludeCount
		{
			get{return m_IncludeStack.Count;}
		}
		
	}
}
