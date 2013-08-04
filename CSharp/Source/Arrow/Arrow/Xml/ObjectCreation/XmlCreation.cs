using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Arrow.Xml.ObjectCreation
{
	/// <summary>
	/// Provides a useful set of methods to create objects via xml
	/// without having to create an instance of XmlObjectCreation
	/// </summary>
	public static class XmlCreation
	{
		/// <summary>
		/// Creates an object from an xml node which has its "type" attribute set to an encoded type name.
		/// If no "type" attribute is present the object is assumed to be a string
		/// </summary>
		/// <typeparam name="T">The type to cast the created object to</typeparam>
		/// <param name="node">An xml description of the object to create</param>
		/// <exception cref="System.ArgumentNullException">node is null</exception>
		public static T Create<T>(XmlNode node)
		{
			return new CustomXmlCreation().Create<T>(typeof(T),node,null);
		}
				
		/// <summary>
		/// Creates an object from the given node
		/// </summary>
		/// <param name="node">The node to process</param>
		/// <param name="type">The concreate type to create if the node does not specify a type</param>
		/// <returns>The created object</returns>
		public static object Create(Type type, XmlNode node)
		{
			return new CustomXmlCreation().Create<object>(type,node,null);
		}
		
		/// <summary>
		/// Constructs on object of the specified type from the given node.
		/// </summary>
		/// <typeparam name="T">The type that the concrete type must be castable to</typeparam>
		/// <param name="node">The node to process</param>
		/// <param name="type">The concrete type to create. It must derive or implement T</param>
		/// <returns>An instance of the type</returns>
		/// <exception cref="System.ArgumentNullException">node is null</exception>
		/// <exception cref="System.ArgumentNullException">type is null</exception>
		public static T Create<T>(Type type, XmlNode node) 
		{
			return new CustomXmlCreation().Create<T>(type,node,null);
		}
		
				
		/// <summary>
		/// Creates a List from the nodes in the xml node list.
		/// </summary>
		/// <remarks>
		/// The nodes in the list must be creatable to at least type T
		/// </remarks>
		/// <typeparam name="T">The type to store in the list</typeparam>
		/// <param name="nodes">The nodes to process</param>
		/// <returns>A list</returns>
		/// <exception cref="System.ArgumentNullException">nodes is null</exception>
		public static List<T> CreateList<T>(XmlNodeList nodes)
		{
			return new CustomXmlCreation().CreateList<T>(nodes);
		}
				
		
		/// <summary>
		/// Applies an xml node to an existing object.
		/// </summary>
		/// <remarks>
		/// This is useful if you have already created an instance and wish to set properties and call methods on it
		/// </remarks>
		/// <param name="objectNode">The xml node to apply</param>
		/// <param name="object">The object to apply to</param>
		public static void Apply(object @object, XmlNode objectNode)
		{
			new CustomXmlCreation().Apply(@object,objectNode);
		}

		/// <summary>
		/// Applies a sequence of nodes to an existing object
		/// </summary>
		/// <param name="object">The object to apply to</param>
		/// <param name="nodes">The nodes to apply</param>
		public static void Apply(object @object, IEnumerable<XmlNode> nodes)
		{
			var creation=new CustomXmlCreation();
			creation.ApplyNodes(@object,nodes);
		}

		/// <summary>
		/// Returns a class that can be used to create instances of an object at a later stage
		/// </summary>
		/// <typeparam name="T">The minimum type the created type must be</typeparam>
		/// <param name="node">The node containing an xml description of the object</param>
		/// <returns>A DelayedCreator instance for the type</returns>
		/// <exception cref="System.ArgumentNullException">node is null</exception>
		/// <exception cref="System.InvalidCastException">the delayed type is not assignable to T</exception>
		public static DelayedCreator DelayedCreate<T>(XmlNode node)
		{
			var creation=new CustomXmlCreation();
			return creation.DelayedCreate<T>(node);
		}
	
		/// <summary>
		/// Create a type from the type information on a node.
		/// This is the prefered method of extracting type information from xml.
		/// If a type attribute is not present then the string type is returned.
		/// </summary>
		/// <param name="node">The node to extract type information from</param>
		/// <returns>A type</returns>
		/// <exception cref="System.ArgumentNullException">node is null</exception>
		public static Type CreateType(XmlNode node)
		{
			var creation=new CustomXmlCreation();
			return creation.CreateType(node);
		}
	}
}
