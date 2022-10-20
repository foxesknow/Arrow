using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Reflection;

using Arrow.Reflection;

#nullable disable

namespace Arrow.Xml.ObjectCreation
{
	public partial class CustomXmlCreation
	{
		/// <summary>
		/// Handles populating a generic collection property
		/// </summary>
		/// <param name="theObject">The object we're processing</param>
		/// <param name="node">The node whose children will be added to the list</param>
		/// <param name="propertyInfo">The property that has the generic collection we're going to populate</param>
		/// <remarks>
		/// <para>For every child node of "node" a call is made to CreateObject and the object added to the list</para>
		/// </remarks>
		private void ProcessGenericCollectionProperty(object theObject, XmlNode node, PropertyInfo propertyInfo)
		{
			// We need to get the collection
			MethodInfo getter=propertyInfo.GetGetMethod();
			if(getter==null) throw new XmlCreationException("could not find property getter: "+propertyInfo.Name);
			
			object collection=getter.Invoke(theObject,null);
			if(collection==null) throw new XmlCreationException("property returned a null collection: "+propertyInfo.Name);
			
			foreach(XmlNode containedNode in node.SelectNodes("*"))
			{
				ProcessGenericCollection(collection,containedNode);
			}
		}
		
		private void ProcessGenericCollection(object collection, XmlNode item)
		{
			Type collectionType=collection.GetType();
			Type containedType=typeof(object);
			
			if(collectionType.IsGenericType)
			{
				Type[] genericTypes=collectionType.GetGenericArguments();
				containedType=genericTypes[0];
			}
			
			MethodInfo addMethod=collection.GetType().GetMethod("Add",new Type[]{containedType});
			if(addMethod==null) throw new XmlCreationException("could not find Add method");
			
			object obj=CreateObject(item,containedType);
			obj=TypeResolver.CoerceToType(containedType,obj);
			addMethod.Invoke(collection,new object[]{obj});
		}
		
		/// <summary>
		/// Handles populating via the IValueAdder interface 
		/// </summary>
		/// <param name="theObject"></param>
		/// <param name="node"></param>
		/// <param name="propertyInfo"></param>
		private void ProcessValueAdderProperty(object theObject, XmlNode node, PropertyInfo propertyInfo)
		{
			// We need to get the collection
			MethodInfo getter=propertyInfo.GetGetMethod();
			if(getter==null) throw new XmlCreationException("could not find property getter: "+propertyInfo.Name);
			
			object collection=getter.Invoke(theObject,null);
			if(collection==null) throw new XmlCreationException("property returned a null collection: "+propertyInfo.Name);
			
			Type propertyType=propertyInfo.PropertyType;
			Type[] genericTypes=propertyType.GetGenericArguments();
			Type containedType=genericTypes[0];
			MethodInfo addMethod=null;
			
			addMethod=collection.GetType().GetMethod("Add",new Type[]{containedType});
			if(addMethod==null) throw new XmlCreationException("could not find Add method");
			
			foreach(XmlNode containedNode in node.SelectNodes("*"))
			{
				object obj=CreateObject(containedNode,containedType);
				obj=TypeResolver.CoerceToType(containedType,obj);
				addMethod.Invoke(collection,new object[]{obj});
			}
		}
		
		/// <summary>
		/// Handles populating a list property
		/// </summary>
		/// <param name="theObject">The object we're processing</param>
		/// <param name="node">The node whose children will be added to the list</param>
		/// <param name="propertyInfo">The property that has the list we're going to populate</param>
		/// <remarks>
		/// <para>For every child node of "node" a call is made to CreateObject and the object added to the list</para>
		/// <para>
		/// If the list is a generic list then the type of object it holds is used to indicate the 
		/// expected type in the xml, otherwise it is assumed to be an object instance
		/// </para>
		/// </remarks>
		private void ProcessListProperty(object theObject, XmlNode node, PropertyInfo propertyInfo)
		{
			// We need to get the collection
			MethodInfo getter=propertyInfo.GetGetMethod();
			if(getter==null) throw new XmlCreationException("could not find property getter: "+propertyInfo.Name);
			
			object list=getter.Invoke(theObject,null);
			if(list==null) throw new XmlCreationException("property returned a null list: "+propertyInfo.Name);
			
			foreach(XmlNode item in node.SelectNodes("*"))
			{
				ProcessList(list,item);
			}
		}
		
		private void ProcessList(object list, XmlNode item)
		{
			Type listType=list.GetType();
			Type containedType=typeof(object);
			MethodInfo addMethod=null;
			
			if(listType.IsGenericType)
			{
				Type[] genericTypes=listType.GetGenericArguments();
				containedType=genericTypes[0];
			}
			
			addMethod=listType.GetMethod("Add",new Type[]{containedType});
			if(addMethod==null) throw new XmlCreationException("could not find Add method");
			
			object obj=CreateObject(item,containedType);
			obj=TypeResolver.CoerceToType(containedType,obj);
			addMethod.Invoke(list,new object[]{obj});
		}
		
		/// <summary>
		/// Populates a dictionary
		/// </summary>
		/// <param name="theObject">The object we're processing</param>
		/// <param name="node">The node whose children will be added to the dictionary</param>
		/// <param name="propertyInfo">The property that has the dictionary we're going to populate</param>
		/// <remarks>
		/// <para>
		/// Every child of "node" must contain a "key" and "value" pair that will be used to populate the dictionary.
		/// </para>
		/// <para>
		/// If the dictionary is a generic dictionary then the type of key and value it holds is used to indicate the 
		/// expected type in the xml, otherwise it is assumed to be an object to object dictionary instance
		/// </para>
		/// </remarks>
		private void ProcessDictionaryProperty(object theObject, XmlNode node, PropertyInfo propertyInfo)
		{
			// We need to get the collection
			MethodInfo getter=propertyInfo.GetGetMethod();
			if(getter==null) throw new XmlCreationException("could not find property getter: "+propertyInfo.Name);
			
			object dictionary=getter.Invoke(theObject,null);
			if(dictionary==null) throw new XmlCreationException("property returned a null list: "+propertyInfo.Name);
			
			foreach(XmlNode pairNode in node.SelectNodes("*"))
			{
				ProcessDictionary(dictionary,pairNode);
			}
		}
		
		private void ProcessDictionary(object dictionary, XmlNode pairNode)
		{
			// We'll assume it's an untyped dictionary (like a Hashtable)
			Type dictionaryType=dictionary.GetType();
			Type keyType=typeof(object);
			Type valueType=typeof(object);
			
			if(dictionaryType.IsGenericType)
			{
				Type[] genericTypes=dictionaryType.GetGenericArguments();
				keyType=genericTypes[0];
				valueType=genericTypes[1];
			}
			
			MethodInfo addMethod=dictionary.GetType().GetMethod("Add",new Type[]{keyType,valueType});
			if(addMethod==null) throw new XmlCreationException("could not find Add method");
			
			XmlNode keyNode=pairNode.SelectSingleNode("Key|@key");
			if(keyNode==null) throw new XmlCreationException("dictionary entry needs a key");
			object key=CreateObject(keyNode,keyType);
			key=TypeResolver.CoerceToType(keyType,key);
			
			XmlNode valueNode=pairNode.SelectSingleNode("Value|@value");
			if(valueNode==null) throw new XmlCreationException("dictionary entry needs a value");
			object value=CreateObject(valueNode,valueType);
			value=TypeResolver.CoerceToType(valueType,value);
			
			addMethod.Invoke(dictionary,new object[]{key,value});
		}
		
		/// <summary>
		/// Navigates down through the supertypes on an object looking for a generic type implementation
		/// </summary>
		/// <param name="objectType"></param>
		/// <param name="genericType"></param>
		/// <returns></returns>
		private bool IsGenericTypeImplemented(Type objectType, Type genericType)
		{
			if(objectType.IsGenericType && objectType.GetGenericTypeDefinition()==genericType) return true;
		
			bool implemented=false;
			
			Type[] interfaces=objectType.GetInterfaces();
			for(int i=0; i<interfaces.Length && implemented==false; i++)
			{
				Type type=interfaces[i];
				implemented=(type.IsGenericType && type.GetGenericTypeDefinition()==genericType);
			}
			
			return implemented;
		}
		
		private bool IsList(Type type)
		{
			return typeof(System.Collections.IList).IsAssignableFrom(type) || IsGenericTypeImplemented(type,typeof(IList<>));
		}
		
		private bool IsDictionary(Type type)
		{
			return typeof(System.Collections.IDictionary).IsAssignableFrom(type) || IsGenericTypeImplemented(type,typeof(IDictionary<,>));
		}
		
		private bool IsGenericCollection(Type type)
		{
			return IsGenericTypeImplemented(type,typeof(ICollection<>));
		}
	}
}
