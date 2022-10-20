using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Arrow.Xml.ObjectCreation
{
	/// <summary>
	/// Delays creates an instance of an object froman xml description until required
	/// </summary>
	public class DelayedCreator
	{
		private CustomXmlCreation m_Factory;
		private XmlNode m_Node;
		private Type m_Type;
		private Uri? m_BaseUri;
	
		/// <summary>
		/// Initializes the object
		/// </summary>
		/// <param name="factory">The factory that created the instance</param>
		/// <param name="node">The node containing the xml description of the object</param>
		/// <param name="type">The type of the object that will be created</param>
		/// <param name="uri">The uri to the document containing the node, if applicable</param>
		internal DelayedCreator(CustomXmlCreation factory, XmlNode node, Type type, Uri? uri)
		{
			m_Factory=factory;
			m_Node=node;
			m_Type=type;
			m_BaseUri=uri;
		}
		
		/// <summary>
		/// The concrete type of the object that will be created
		/// </summary>
		/// <value>The type of object that Create will return</value>
		public Type UnderlyingType
		{
			get{return m_Type;}
		}
	
		/// <summary>
		/// Creates the object by calling <code>ObjectCreation.Create</code>
		/// </summary>
		/// <typeparam name="T">A type that the underlying type is castable to</typeparam>
		/// <returns>An instance derived from the xml description</returns>
		public T Create<T>()
		{
			return m_Factory.Create<T>(m_Type,m_Node,m_BaseUri);
		}
	}
}
