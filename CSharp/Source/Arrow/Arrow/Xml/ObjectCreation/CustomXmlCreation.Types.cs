using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Reflection;
using System.ComponentModel;

using Arrow.Collections;
using Arrow.Reflection;

namespace Arrow.Xml.ObjectCreation
{
	public partial class CustomXmlCreation
	{
		internal static readonly string Typename="type";
		
		/// <summary>
		/// Parses an enum value from a string
		/// </summary>
		/// <typeparam name="T">The type of the enum</typeparam>
		/// <param name="value">The value to parse</param>
		/// <returns>A value from the enumeration</returns>
		/// <exception cref="System.ArgumentException">value is null</exception>
		public static T ParseEnum<T>(string value)
		{
			if(value==null) throw new ArgumentNullException("value");
			
			return (T)TypeResolver.ExpandEnum(typeof(T),value);
		}
		
		/// <summary>
		/// Extracts the type attribute from a node.
		/// Currently both namespaced and non-namespaced attributes are supported.
		/// However, the non-namespace support is deprecated and will be removed
		/// </summary>
		/// <param name="node">The node to check for a type attribute</param>
		/// <returns>The type attribute, or null if one does not exist</returns>
		internal static XmlAttribute GetTypeAttribute(XmlNode node)
		{
			XmlAttribute typeNode=(node.Attributes[Typename,FactoryNS] ?? node.Attributes[Typename]);
			return typeNode;
		}
			
		private static Type HandleGenerics(XmlNode node, Type type)
		{
			if(type.IsGenericTypeDefinition)
			{
				XmlNode genericsNode=node["Generics",FactoryNS];
				if(genericsNode==null) throw new XmlCreationException("generic types require a generics element");
				
				type=BuildGenericType(type,genericsNode);
			}
			
			return type;
		}
	
		private static Type BuildGenericType(Type baseGenericType, XmlNode node)
		{
			List<Type> types=new List<Type>();
			
			XmlNamespaceManager ns=new XmlNamespaceManager(new NameTable());
			ns.AddNamespace("obj",FactoryNS);
			
			// We need to extract the generic arguments
			foreach(XmlNode generic in node.SelectNodes("obj:Generic",ns))
			{
				XmlNode typeNode=GetTypeAttribute(generic);
				if(typeNode==null) throw new XmlCreationException("generic type required type element");
				Type genericArgument=XmlTypeResolver.GetEncodedType(typeNode);
				
				// This allows us to nest generics
				if(genericArgument.IsGenericTypeDefinition)
				{
					genericArgument=BuildGenericType(genericArgument,generic);
				}
				
				types.Add(genericArgument);
			}
			
			// Make sure we've got enough generic arguments
			int requiredArgs=baseGenericType.GetGenericArguments().Length;
			if(requiredArgs!=types.Count)
			{
				throw new XmlCreationException("not enough/too many generic arguments passed");
			}
			
			Type concreteType=baseGenericType.MakeGenericType(types.ToArray());
			return concreteType;
		}
	}
}
