using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using Arrow.Reflection;

namespace Arrow.Xml.ObjectCreation
{
	/// <summary>
	/// Provides type resolution from xml nodes.
	/// This consists of allowing the user to explicitly name a type or define a namespace that maps to a CLR assembly.
	/// For example: <b>xmlns:s="clr-namespace:System"</b> or <b>xmlns:e="clr-namespace:Arrow.Text;assembly=Arrow"</b>
	/// </summary>
	public static class XmlTypeResolver
	{
		private static readonly string ClrNamespace="clr-namespace:";
		private static readonly string AssemblyPrefix="assembly=";
		
		/// <summary>
		/// Returns the type represented by the specified xml node
		/// </summary>
		/// <param name="typeNode">The node to extract the type from</param>
		/// <returns>A type instance</returns>
		/// <exception cref="System.ArgumentNullException">typeNode is null</exception>
		public static Type GetEncodedType(XmlNode typeNode)
		{
			if(typeNode==null) throw new ArgumentNullException("typeNode");
			
			string typeName=ExtractTypeName(typeNode,typeNode.Value!);
			return TypeResolver.GetEncodedType(typeName);
		}
	
		/// <summary>
		/// Extracts the type information from a node.
		/// The type is either explicitly names (Namespace.Class,MyAssembly)
		/// or indirected via a namespace (t:Class)
		/// </summary>
		/// <param name="typeNode">The node to extract type information from</param>
		/// <returns>The name of the type</returns>
		/// <exception cref="System.ArgumentNullException">typeNode is null</exception>
		public static string ExtractTypeName(XmlNode typeNode)
		{
			return ExtractTypeName(typeNode,typeNode.Value!);
		}		
			
		/// <summary>
		/// Extracts the type information from a node.
		/// The type is either explicitly names (Namespace.Class,MyAssembly)
		/// or indirected via a namespace (t:Class)
		/// </summary>
		/// <param name="namespaceLookupNode">The node to use for namespace lookup</param>
		/// <param name="typeName">The type to resolve</param>
		/// <returns>The type to use in encoded format</returns>
		/// <exception cref="System.ArgumentNullException">namespaceLookupNode is null</exception>
		/// <exception cref="System.ArgumentNullException">typeName is null</exception>
		public static string ExtractTypeName(XmlNode namespaceLookupNode, string typeName)
		{
			if(namespaceLookupNode==null) throw new ArgumentNullException("namespaceLookupNode");
			if(typeName==null) throw new ArgumentNullException("typeName");
			
			int pivot=typeName.IndexOf(':');
			if(pivot!=-1)
			{
				// It's a typename marked up my an xml namespace
				string prefix=typeName.Substring(0,pivot);
				string className=typeName.Substring(pivot+1);
				
				// Make sure the namespace looks valid
				string xmlNS=namespaceLookupNode.GetNamespaceOfPrefix(prefix);				
				if(xmlNS.StartsWith(ClrNamespace)==false) throw new ArrowException(prefix+" is not a valid clr-namespace");
				
				typeName=BuildClrNamespaceEncodedType(xmlNS,className);
			}
			
			return typeName;
		}
	
		/// <summary>
		/// Takes a clr-namespace string, such as clr-namespace:System;assembly=mscorlib
		/// and returns an encoded type string
		/// </summary>
		/// <param name="xmlNS">The namespace to parse</param>
		/// <param name="typeName">The type that will be referenced within the namespace</param>
		/// <returns>An encoded string for the type</returns>
		private static string BuildClrNamespaceEncodedType(string xmlNS, string typeName)
		{
			string descriptor=xmlNS.Substring(ClrNamespace.Length);
			
			string[] parts=descriptor.Split(new char[]{';'},2);
			if(parts.Length==0) throw new ArrowException("invalid namespace: "+xmlNS);
			
			string @namespace=parts[0].Trim();
			string encodedType=string.Format("{0}.{1}",@namespace,typeName);
			
			if(parts.Length==2)
			{
				// There's a reference to an assembly
				string assemblyName=parts[1];
				if(assemblyName.StartsWith(AssemblyPrefix)==false) throw new ArrowException("invalid namespace: "+xmlNS);
				
				assemblyName=assemblyName.Substring(AssemblyPrefix.Length).Trim();
				if(assemblyName=="") throw new ArrowException("invalid namespace: "+xmlNS);
				
				encodedType=string.Format("{0},{1}",encodedType,assemblyName);
			}
			
			return encodedType;
		}
	}
}
