using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.ComponentModel;
using System.Xml;
using System.Linq;

using Arrow.Storage;
using Arrow.Settings;
using Arrow.Collections;
using Arrow.Reflection;
using Arrow.Text;
using Arrow.ObjectAccess;

namespace Arrow.Xml.ObjectCreation
{
	/// <summary>
	/// Creates and initializes object from an xml description.
	/// </summary>
	/// <remarks>
	/// <para>In addition to setting properties on an object you can also call methods on the object.</para>
	/// <para>
	/// The factory can use the AccessManager to reference definitions stored elsewhere.
	/// To do so create a reference to the "urn:Arrow.xml.objectcreation" namespace (eq xmlns:obj="urn:Arrow.xml.objectcreation")
	/// and use obj:url to reference external xml. The url must contain a scheme registered with the
	/// AccessFactory. By default the root element of the returned xml will be used to construct the object.
	/// You can override this by setting obj:select to select a single element within the document.
	/// </para>
	/// <para>
	/// Properties and methods that you wish to access must be public. 
	/// it is not an error to try and set a property that does not exist.
	/// </para>
	/// </remarks>
	public partial class CustomXmlCreation
	{
		internal static readonly string BeginToken=TokenExpander.DefaultBeginToken;
		internal static readonly string EndToken=TokenExpander.DefaultEndToken;
		
		private static readonly BindingFlags PropertyBindings=BindingFlags.Public|BindingFlags.IgnoreCase|BindingFlags.Instance;
		
		private static readonly BindingFlags MethodInstanceBindings=BindingFlags.Public|BindingFlags.IgnoreCase|BindingFlags.Instance;
		private static readonly BindingFlags MethodStaticBindings=BindingFlags.Public|BindingFlags.IgnoreCase|BindingFlags.Static;
		
		/// <summary>
		/// The namespace for factory specific elements and attributes
		/// </summary>
		public static readonly string FactoryNS="urn:arrow.xml.objectcreation";
		
		private static readonly Type DefaultNodeType=typeof(string);
		
		private Dictionary<string,Converter<Uri,object>> m_ReferenceResolvers=new Dictionary<string,Converter<Uri,object>>();
		
		private Stack<Uri> m_Contexts=new Stack<Uri>();
		
		private Func<string,object> m_UnknownVariableLookup;
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		public CustomXmlCreation() : this(null)
		{
		}
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="unknownVariableLookup">A delegate that can map a variable to an object during text expansion</param>
		public CustomXmlCreation(Func<string,object> unknownVariableLookup)
		{
			m_UnknownVariableLookup=unknownVariableLookup;
		}
		
		/// <summary>
		/// Creates an object from an xml node which has its "type" attribute set to an encoded type name.
		/// If no "type" attribute is present the object is assumed to be a string
		/// </summary>
		/// <typeparam name="T">The type to cast the created object to</typeparam>
		/// <param name="node">An xml description of the object to create</param>
		/// <exception cref="System.ArgumentNullException">node is null</exception>
		/// <remarks>
		/// <para>
		/// Arguments are passed to the constructor my placing children under node.
		/// The names of the elements used for constructor arguments are irrelevant but should be reasonably meaningful.
		/// After construction any child elements are applied to the object, either as properties or method calls.
		/// </para>
		/// <para>
		/// If an exception is thrown after construction then <code>IDisposable.Dispose</code> will be called
		/// on any created object if it is implemented
		/// </para>
		/// </remarks>
		public T Create<T>(XmlNode node)
		{
			return Create<T>(typeof(T),node,null);
		}
		
		/// <summary>
		/// Creates an object from an xml node which has its "type" attribute set to an encoded type name.
		/// If no "type" attribute is present the object is assumed to be a string
		/// </summary>
		/// <typeparam name="T">The type to cast the created object to</typeparam>
		/// <param name="node">The node to process</param>
		/// <param name="baseLocation">The base uri to use when resolving urls. May be null</param>
		/// <returns>The created object</returns>
		/// <exception cref="System.ArgumentNullException">node is null</exception>
		public T Create<T>(XmlNode node, Uri baseLocation)
		{
			return Create<T>(typeof(T),node,baseLocation);
		}
		
		/// <summary>
		/// Creates an object from the given node
		/// </summary>
		/// <param name="node">The node to process</param>
		/// <param name="type">The concreate type to create if the node does not specify a type</param>
		/// <returns>The created object</returns>
		/// <exception cref="System.ArgumentNullException">node is null</exception>
		public object Create(Type type, XmlNode node)
		{
			return Create<object>(type,node,null);
		}
		
		/// <summary>
		/// Creates an object from the given node
		/// </summary>
		/// <param name="node">The node to process</param>
		/// <param name="type">The concreate type to create if the node does not specify a type</param>
		/// <param name="baseLocation">The base uri to use when resolving urls. May be null</param>
		/// <returns>The created object</returns>
		/// <exception cref="System.ArgumentNullException">node is null</exception>
		public object Create(Type type, XmlNode node, Uri baseLocation)
		{
			return Create<object>(type,node,baseLocation);
		}
		
		/// <summary>
		/// Creates a List from the nodes in the xml node list.
		/// </summary>
		/// <typeparam name="T">The type to store in the list</typeparam>
		/// <param name="nodes">The nodes to process</param>
		/// <returns>A list</returns>
		/// <exception cref="System.ArgumentNullException">nodes is null</exception>
		public List<T> CreateList<T>(XmlNodeList nodes)
		{
			return CreateList<T>(nodes,null);
		}
				
		/// <summary>
		/// Creates a List from the nodes in the xml node list.
		/// </summary>
		/// <typeparam name="T">The type to store in the list</typeparam>
		/// <param name="nodes">The nodes to process</param>
		/// <param name="baseLocation">The base uri to use when resolving urls. May be null</param>
		/// <returns>A list</returns>
		/// <exception cref="System.ArgumentNullException">nodes is null</exception>
		public List<T> CreateList<T>(XmlNodeList nodes, Uri baseLocation)
		{
			if(nodes==null) throw new ArgumentNullException("nodes");
		
			List<T> list=new List<T>();
			
			foreach(XmlNode node in nodes)
			{
				T item=Create<T>(typeof(T),node,baseLocation);
				list.Add(item);
			}
			
			return list;
		}


		/// <summary>
		/// Creates a List from the nodes in the xml node list.
		/// </summary>
		/// <typeparam name="T">The type to store in the list</typeparam>
		/// <param name="nodes">The nodes to process</param>
		/// <returns>A list</returns>
		/// <exception cref="System.ArgumentNullException">nodes is null</exception>
		public List<T> CreateList<T>(IEnumerable<XmlNode> nodes)
		{
			return CreateList<T>(nodes,null);
		}
				
		/// <summary>
		/// Creates a List from the nodes in the xml node list.
		/// </summary>
		/// <typeparam name="T">The type to store in the list</typeparam>
		/// <param name="nodes">The nodes to process</param>
		/// <param name="baseLocation">The base uri to use when resolving urls. May be null</param>
		/// <returns>A list</returns>
		/// <exception cref="System.ArgumentNullException">nodes is null</exception>
		public List<T> CreateList<T>(IEnumerable<XmlNode> nodes, Uri baseLocation)
		{
			if(nodes==null) throw new ArgumentNullException("nodes");
		
			List<T> list=new List<T>();
			
			foreach(XmlNode node in nodes)
			{
				T item=Create<T>(typeof(T),node,baseLocation);
				list.Add(item);
			}
			
			return list;
		}

				/// <summary>
		/// Returns a class that can be used to create instances of an object at a later stage
		/// </summary>
		/// <typeparam name="T">The minimum type the created type must be</typeparam>
		/// <param name="node">The node containing an xml description of the object</param>
		/// <returns>A DelayedCreator instance for the type</returns>
		/// <exception cref="System.ArgumentNullException">node is null</exception>
		/// <exception cref="System.InvalidCastException">the delayed type is not assignable to T</exception>
		public DelayedCreator DelayedCreate<T>(XmlNode node)
		{
			return DelayedCreate<T>(node,null);
		}
				
		/// <summary>
		/// Returns a class that can be used to create instances of an object at a later stage
		/// </summary>
		/// <typeparam name="T">The minimum type the created type must be</typeparam>
		/// <param name="node">The node containing an xml description of the object</param>
		/// <param name="baseLocation">The base uri to use when resolving urls. May be null</param>
		/// <returns>A DelayedCreator instance for the type</returns>
		/// <exception cref="System.ArgumentNullException">node is null</exception>
		/// <exception cref="System.InvalidCastException">the delayed type is not assignable to T</exception>
		public DelayedCreator DelayedCreate<T>(XmlNode node, Uri baseLocation)
		{
			if(node==null) throw new ArgumentNullException("node");
			
			Type type=CreateType(node);
				
			if(typeof(T).IsAssignableFrom(type)==false)
			{
				throw new InvalidCastException();
			}
			
			return new DelayedCreator(this,node,type,baseLocation);
		}

		
		/// <summary>
		/// Populates a dictionary (by calls to Add) with data.
		/// The name of the element/attribute is used as they key
		/// </summary>
		/// <typeparam name="T">The minimum type required</typeparam>
		/// <param name="dictionary">The dictionary to populate</param>
		/// <param name="nodes">The nodes to process</param>
		/// <exception cref="System.ArgumentNullException">dictionary is null</exception>
		/// <exception cref="System.ArgumentNullException">nodes is null</exception>
		public void PopulateDictionary<T>(IDictionary<string,T> dictionary, XmlNodeList nodes)
		{
			PopulateDictionary<T>(dictionary,nodes,null);
		}
		
		/// <summary>
		/// Populates a dictionary (by calls to Add) with data.
		/// The name of the element/attribute is used as they key
		/// </summary>
		/// <typeparam name="T">The minimum type required</typeparam>
		/// <param name="dictionary">The dictionary to populate</param>
		/// <param name="nodes">The nodes to process</param>
		/// <param name="baseLocation">The base uri to use when resolving urls. May be null</param>
		/// <exception cref="System.ArgumentNullException">dictionary is null</exception>
		/// <exception cref="System.ArgumentNullException">nodes is null</exception>
		public void PopulateDictionary<T>(IDictionary<string,T> dictionary, XmlNodeList nodes, Uri baseLocation)
		{
			if(dictionary==null) throw new ArgumentNullException("dictionary");
			if(nodes==null) throw new ArgumentNullException("nodes");
			
			foreach(XmlNode node in nodes)
			{
				string name=node.Name;
				T item=Create<T>(typeof(T),node,baseLocation);
				dictionary.Add(name,item);
			}
		}
		
		/// <summary>
		/// Populates a dictionay from a list of key/value pairs
		/// </summary>
		/// <typeparam name="K">The type of the key</typeparam>
		/// <typeparam name="V">The type of the value</typeparam>
		/// <param name="dictionary">The dictionary to populate</param>
		/// <param name="nodes">The nodes to process</param>
		public void PopulateKeyValuePair<K,V>(IDictionary<K,V> dictionary, XmlNodeList nodes)
		{
			if(dictionary==null) throw new ArgumentNullException("dictionary");
			if(nodes==null) throw new ArgumentNullException("nodes");
		
			foreach(XmlNode keyValueNode in nodes)
			{
				ProcessDictionary(dictionary,keyValueNode);
			}
		}
		
		/// <summary>
		/// Create a type from the type information on a node.
		/// This is the prefered method of extracting type information from xml.
		/// If a type attribute is not present then the string type is returned.
		/// </summary>
		/// <param name="node">The node to extract type information from</param>
		/// <returns>A type</returns>
		/// <exception cref="System.ArgumentNullException">node is null</exception>
		public Type CreateType(XmlNode node)
		{
			if(node==null) throw new ArgumentNullException("node");
			
			XmlAttribute typeNode=GetTypeAttribute(node);
			if(typeNode==null) return DefaultNodeType;
			
			// The user has specified a different type
			Type type=XmlTypeResolver.GetEncodedType(typeNode);
			type=HandleGenerics(node,type);
			
			return type;
		}
				
		/// <summary>
		/// Applies an xml node to an existing object.
		/// </summary>
		/// <remarks>
		/// This is useful if you have already created an instance and wish to set properties and call methods on it
		/// </remarks>
		/// <param name="objectNode">The xml node to apply</param>
		/// <param name="object">The object to apply to</param>
		/// <exception cref="System.ArgumentNullException">objectNode is null</exception>
		/// <exception cref="System.ArgumentNullException">object is null</exception>
		public void Apply(object @object, XmlNode objectNode)
		{
			if(objectNode==null) throw new ArgumentNullException("objectNode");
			if(@object==null) throw new ArgumentNullException("object");
			
			ApplyPropertiesAndMethods(@object,@object.GetType(),objectNode);
		}

		/// <summary>
		/// Applies the attribtes of a node as properties to an object
		/// </summary>
		/// <param name="node">The node to apply</param>
		/// <param name="object">The object to apply the properties to</param>
		/// <exception cref="System.ArgumentNullException">node is null</exception>
		/// <exception cref="System.ArgumentNullException">object is null</exception>
		public void ApplyNodeAttributes(object @object, XmlNode node)
		{
			if(node==null) throw new ArgumentNullException("node");
			if(@object==null) throw new ArgumentNullException("object");
			
			var attributeNodes=node.Attributes.Cast<XmlNode>();

			ApplyNodes(@object,attributeNodes);
		}
		
		/// <summary>
		/// Applies the initialization cycle to an object (ISupportInitialize, IObjectInitialization, properties and methods)
		/// </summary>
		/// <param name="objectNode">The xml node to apply</param>
		/// <param name="object">The object to apply to</param>
		/// <exception cref="System.ArgumentNullException">objectNode is null</exception>
		/// <exception cref="System.ArgumentNullException">object is null</exception>
		public void InitializeInstance(object @object, XmlNode objectNode)
		{
			if(objectNode==null) throw new ArgumentNullException("objectNode");
			if(@object==null) throw new ArgumentNullException("object");
			
			DoInitializeInstance(@object,@object.GetType(),objectNode);
		}
				
		internal Dictionary<string,Converter<Uri,object>> ReferenceResolvers
		{
			get{return m_ReferenceResolvers;}
		}
		
		/// <summary>
		/// Constructs on object of the specified type from the given node.
		/// </summary>
		/// <typeparam name="T">The type that the concrete type must be castable to</typeparam>
		/// <param name="node">The node to process</param>
		/// <param name="type">The concrete type to create. It must derive or implement T</param>
		/// <param name="baseLocation">The base uri to use when resolving urls. May be null</param>
		/// <returns>An instance of the type</returns>
		/// <exception cref="System.ArgumentNullException">node is null</exception>
		/// <exception cref="System.ArgumentNullException">type is null</exception>
		/// <remarks>
		/// If the node does not contain a "type" attribute then the type parameter will be used.
		/// This method is useful when the concrete type is different from the generic type
		/// If an exception is thrown after construction then <code>IDisposable.Dispose</code> will be called
		/// on any created object if it is implemented
		/// </remarks>
		public T Create<T>(Type type, XmlNode node, Uri baseLocation) 
		{
			if(node==null) throw new ArgumentNullException("node");
			if(type==null) throw new ArgumentNullException("type");
			
			object obj=null;
			try
			{
				if(baseLocation!=null) m_Contexts.Push(baseLocation);
				
				obj=CreateObject(node,type);
				return (T)obj;
			}
			catch
			{
				if(baseLocation!=null) m_Contexts.Pop();
				
				IDisposable disposable=obj as IDisposable;
				if(disposable!=null) disposable.Dispose();
				throw;
			}			
		}
		
		/// <summary>
		/// Creates an object from an xml node
		/// </summary>
		/// <param name="objectNode">The xml node containing the object description</param>
		/// <param name="type">The type of the object to create if the node does not specify a type</param>
		private object CreateObject(XmlNode objectNode, Type type)
		{
			object obj=null;
			
			// See if the object node has overridden the actual type
			if(objectNode.Attributes!=null)
			{
				XmlAttribute typeofNode=GetTypeofAttribute(objectNode);
				if(typeofNode!=null)
				{
					// The user is interested in getting a System.Type instance,
					// rather than creating an instance of the specified type
					return XmlTypeResolver.GetEncodedType(typeofNode);
				}

				// NOTE: We support type in or out of the namespace.
				// The non-namespace support is deprecated and will be removed
				XmlAttribute typeNode=GetTypeAttribute(objectNode);
				if(typeNode!=null)
				{
					// The user has specified a different type
					type=XmlTypeResolver.GetEncodedType(typeNode);
				}
			}
			
			type=HandleGenerics(objectNode,type);
			
			if(objectNode.NodeType==XmlNodeType.Attribute)
			{
				// The attribute value is the object
				string data=objectNode.InnerText;
				data=ExpandText(data);
				return CreateFromString(objectNode,type,data);				
			}
			
			XmlElement element=objectNode as XmlElement;			
			XmlAttribute urlAttr=(element==null ? null : element.Attributes["uri",FactoryNS]);
			XmlAttribute objectAttr=(element==null ? null : element.Attributes["object",FactoryNS]);
			
			if(objectAttr!=null)
			{
				// The object is a direct reference to another object somewhere in the system
				obj=ReferenceExistingObject(type,objectAttr.Value,objectAttr);
				
				// NOTE: Since we're referencing an existing object we must not
				// do anything to it (as we didn't create it)
				return obj;
			}
			else if(urlAttr!=null)
			{
				obj=CreateFromExternalSource(type,urlAttr.Value,element);
			}
			else if(element!=null && element.IsEmpty)
			{
				// It's a call to the default constructor
				obj=CreateInstance(type,null);
			}
			else if(element!=null && element.SelectNodes("*").Count==0)
			{
				// It's an element which just contains text
				string data=objectNode.InnerText;
				if(IsExpansionRequired(element)) data=ExpandText(data);
				return CreateFromString(objectNode,type,data);
			}
			else
			{
				// It's valid to set something to null
				if(objectNode["Null",FactoryNS]!=null) return null;

				// It's an element with children. Get any constructor elements and create an instance
				XmlNode constructorNode=objectNode["Ctor",FactoryNS];
				object[] constructorParameters=GetConstructorParameters(type,constructorNode);
				obj=CreateInstance(type,constructorParameters);
			}
			
			DoInitializeInstance(obj,type,objectNode);			
			return obj;
		}
		
		/// <summary>
		/// Checks if the user want to enable/disable text expansion within the element.
		/// This overrides any global settings that may be in place
		/// </summary>
		/// <param name="element">The element to examine</param>
		/// <returns>true to text expand the element, false to leave it as is</returns>
		private bool IsExpansionRequired(XmlElement element)
		{
			XmlAttribute expandAttr=(element==null ? null : element.Attributes["expand",FactoryNS]);
			bool expand=true;
			if(expandAttr!=null)
			{
				bool value;
				if(bool.TryParse(expandAttr.Value,out value)) expand=value;
			}
			
			return expand;
		}
		
		/// <summary>
		/// Calls any begin/end initialization methods and properties and methods
		/// </summary>
		/// <param name="obj">The object to initialize</param>
		/// <param name="type">The type to query against</param>
		/// <param name="objectNode">The xml node to use for initialization</param>
		private void DoInitializeInstance(object obj, Type type, XmlNode objectNode)
		{
			ISupportInitialize initializer=obj as ISupportInitialize;
			
			if(initializer!=null) initializer.BeginInit();
			
			// See if the object wants to do the work itself
			ICustomXmlInitialization objectInitialization=obj as ICustomXmlInitialization;			
			if(objectInitialization!=null)
			{
				objectInitialization.InitializeObject(objectNode,this);
			}
			else
			{
				// Process all the child elements. They're either properties or method calls
				ApplyPropertiesAndMethods(obj,type,objectNode);
			}
			
			if(initializer!=null) initializer.EndInit();
		}
		
		/// <summary>
		/// Creates an object from a string description
		/// </summary>
		/// <param name="node">The node (attribute or element) that represents the item</param>
		/// <param name="type">The type that is expected</param>
		/// <param name="value">The string version of the object</param>
		/// <returns>An object of the specified type</returns>
		private object CreateFromString(XmlNode node, Type type, string value)
		{
			return QuickInitialize(TypeResolver.CoerceToType(type,value));
		}
		
		/// <summary>
		/// Grabs an external xml definition of an object and create
		/// an object from that definition.
		/// </summary>
		/// <param name="targetType">The type we expect to get</param>
		/// <param name="url">The url to the object definition</param>
		/// <param name="element">The element that specified the external link</param>
		/// <returns>An object</returns>
		private object CreateFromExternalSource(Type targetType, string url, XmlElement element)
		{
			url=ExpandText(url);
			Uri uri=null;
			
			if(m_Contexts.Count!=0)
			{
				// We're already processing an explicit path, so make sure we resolve relative to it
				Uri currentUri=m_Contexts.Peek();
				uri=Accessor.ResolveRelative(currentUri,url);
			}			
			else
			{
				uri=new Uri(url);
			}
			using(Stream stream=StorageManager.Get(uri).OpenRead())
			{
				XmlDocument doc=new XmlDocument();
				doc.Load(stream);
				
				// The user can either select a node or we'll default to the document element
				XmlNode definitionNode=doc.DocumentElement;
				
				XmlAttribute selectAttr=element.Attributes["select",FactoryNS];
				if(selectAttr!=null)
				{
					string xpath=ExpandText(selectAttr.Value);
					definitionNode=doc.SelectSingleNode(xpath);
					
					if(definitionNode==null)
					{
						throw new XmlCreationException("CreateFromExternalSource - select does not identify a node");
					}					
				}
				
				try
				{
					m_Contexts.Push(uri);
					
					object obj=CreateObject(definitionNode,targetType);
					obj=TypeResolver.CoerceToType(targetType,obj);
					return obj;
				}
				finally
				{
					m_Contexts.Pop();
				}
			}
		}
		
		/// <summary>
		/// References an exisiting object somewhere in the system
		/// </summary>
		/// <param name="type">The type expected</param>
		/// <param name="url">The url to the object</param>
		/// <param name="node">The node that made the reference</param>
		/// <returns></returns>
		private object ReferenceExistingObject(Type type, string url, XmlNode node)
		{
			url=ExpandText(url);
			
			// To avoid really long urls for statics we'll allow the
			// user to write something like static://{g:class}/property
			Func<string,object> namespaceExpansion=typeName=>
			{
				string data=XmlTypeResolver.ExtractTypeName(node,typeName);
				
				// The type,assembly needs to be re-written as type@assembly
				data=data.Replace(',','@');
				return data;
			};
			
			// To the text expansion to patch any type references
			url=TokenExpander.ExpandText(url,"{","}",namespaceExpansion);
			Uri uri=new Uri(url);
			
			object obj=null;
			
			
			if(ObjectLocator.IsSchemeSupported(uri.Scheme))
			{
				obj=ObjectLocator.Locate(uri,null);
			}
			else
			{
				// See if anyone has shown an interest
				Converter<Uri,object> resolver;
				if(m_ReferenceResolvers.TryGetValue(uri.Scheme,out resolver))
				{
					obj=resolver(uri);
				}
				else
				{
					throw new XmlCreationException("unknown object location scheme: "+uri.Scheme);
				}
			}
			
			obj=TypeResolver.CoerceToType(type,obj);
			return obj;
		}
		
		/// <summary>
		/// Calls the ISupportInitialize methods and returns the supplied object
		/// </summary>
		/// <param name="obj">The object to initialize</param>
		/// <returns>The original object</returns>
		private object QuickInitialize(object obj)
		{
			ISupportInitialize initializer=obj as ISupportInitialize;
			
			if(initializer!=null) 
			{
				initializer.BeginInit();
				initializer.EndInit();
			}
			
			return obj;
		}
		
		/// <summary>
		/// Applies propertiy values and calls methods on an object
		/// </summary>
		/// <param name="obj">The object to use</param>
		/// <param name="type">The type to treat the object as</param>
		/// <param name="objectNode">A node containing the properties and methods to apply</param>
		private void ApplyPropertiesAndMethods(object obj, Type type, XmlNode objectNode)
		{
			// Process all the child elements. They're either properties or method calls
			foreach(XmlNode node in objectNode.SelectNodes("*|@*"))
			{
				// Ignore any namespace'd values
				if(node.NamespaceURI!="") continue;
			
				string name=node.Name;
				
				string propertyName=name;
				object propertyObject=obj;
				
				if(ResolveProperty(ref propertyObject,ref propertyName) && IsProperty(propertyObject,propertyName))
				{
					HandleProperty(propertyObject,propertyName,node);
				}
				else if(IsMethod(type,name))
				{
					HandleCall(type,obj,node);
				}
				else
				{
					// No method/property found. This isn't classed as an error
					// since the method/property may have been removed and we don't
					// want to have to change all the xml
				}
			}
		}
		
		private XmlNamespaceManager CreateNamespaceManager()
		{
			XmlNamespaceManager manager=new XmlNamespaceManager(new NameTable());
			manager.AddNamespace("obj",CustomXmlCreation.FactoryNS);
			return manager;
		}
		
		/// <summary>
		/// Just applies the nodes as properties to an object
		/// </summary>
		/// <param name="object">The object to set the properties in</param>
		/// <param name="nodes">The node to process</param>
		public void ApplyNodes(object @object, IEnumerable<XmlNode> nodes)
		{
			if(nodes==null) throw new ArgumentNullException("nodes");
			if(@object==null) throw new ArgumentNullException("object");

			foreach(var node in nodes)
			{
				if(node.NamespaceURI!="") continue;
				
				string propertyName=node.Name;
				object propertyObject=@object;
				
				if(ResolveProperty(ref propertyObject,ref propertyName) && IsProperty(propertyObject,propertyName))
				{
					HandleProperty(propertyObject,propertyName,node);
				}
			}
		}
		
		/// <summary>
		/// Applies a qualified property name (eg Person.Age) to a root object
		/// to end up at the object in question
		/// </summary>
		/// <param name="obj">The object to start from</param>
		/// <param name="name">The qualified name of the property</param>
		/// <returns>true if the name resolved, otherwise false</returns>
		private bool ResolveProperty(ref object obj, ref string name)
		{
			string[] parts=name.Split('.');
			
			bool isProperty=true;
			
			for(int i=0; i<parts.Length-1 && isProperty && obj!=null; i++)
			{
				PropertyInfo property=obj.GetType().GetProperty(parts[i],PropertyBindings);
				if(property==null)
				{
					isProperty=false;
					break;
				}
				
				MethodInfo method=property.GetGetMethod();
				if(method==null)
				{
					isProperty=false;
					break;
				}
				
				obj=method.Invoke(obj,null);
			}
			
			// If we got back null then assume it's not a property
			// as we'll have nothing to call against.
			if(obj==null) isProperty=false;
			if(isProperty) name=parts[parts.Length-1];
			
			return isProperty;
		}
		
		/// <summary>
		/// Creates an instance of a type
		/// </summary>
		/// <param name="type">The type of the instance to create</param>
		/// <param name="constructorParameters">Any parameters to the constructor, or null to call the default constructor</param>
		/// <returns>An instance of the specified type</returns>
		private object CreateInstance(Type type, object[] constructorParameters)
		{
			if(type==typeof(string) && constructorParameters==null) return "";
			return Activator.CreateInstance(type,constructorParameters);
		}
		
		/// <summary>
		/// Determines if name corresponds to a property
		/// </summary>
		/// <param name="object">The object to check</param>
		/// <param name="name">The property to check for</param>
		/// <returns>true is name represents a property, false otherwise</returns>
		private bool IsProperty(object @object, string name)
		{
			PropertyInfo info=@object.GetType().GetProperty(name,PropertyBindings);
			return info!=null;
		}
		
		/// <summary>
		/// Determines if name corresponds to a method
		/// </summary>
		/// <param name="type">The type to check</param>
		/// <param name="name">The method to check for</param>
		/// <returns>true is name represents a mehtod, false otherwise</returns>
		private bool IsMethod(Type type, string name)
		{
			MethodInfo info=type.GetMethod(name,MethodInstanceBindings);
			return info!=null;
		}
		
		/// <summary>
		/// Applies a property node to the object
		/// </summary>
		/// <param name="theObject">The object to apply the property node to</param>
		/// <param name="propertyName">The name of the property to process</param>
		/// <param name="propertyNode">The property node to apply</param>
		private void HandleProperty(object theObject, string propertyName, XmlNode propertyNode)
		{
			if(TryImplicitSequenceAdd(theObject,propertyName,propertyNode))
			{
				// We implicitly added the value, so bail out early
				return;
			}
			
			PropertyInfo propertyInfo=theObject.GetType().GetProperty(propertyName,PropertyBindings);
			Type propertyType=propertyInfo.PropertyType;
			
			if(IsList(propertyType))
			{
				ProcessListProperty(theObject,propertyNode,propertyInfo);
			}
			else if(IsDictionary(propertyType))
			{
				ProcessDictionaryProperty(theObject,propertyNode,propertyInfo);
			}
			else if(IsGenericCollection(propertyType))
			{
				// It's rare to get here as most collections are list and dictionary based
				ProcessGenericCollectionProperty(theObject,propertyNode,propertyInfo);
			}
			else if(propertyType.IsGenericType && typeof(IValueAdder<>).IsAssignableFrom(propertyType.GetGenericTypeDefinition()))
			{
				ProcessValueAdderProperty(theObject,propertyNode,propertyInfo);
			}
			else
			{			
				MethodInfo setter=propertyInfo.GetSetMethod();
				if(setter==null) throw new XmlCreationException("could not find property setter: "+propertyName);
				
				object value=null;
				
				// It feasible that the user just wants to grab the xml we've got
				// instead of trying to create an object from it
				if(typeof(XmlNode).IsAssignableFrom(propertyType))
				{
					value=propertyNode;
				}
				else
				{				
					value=CreateObject(propertyNode,propertyType);
					value=TypeResolver.CoerceToType(propertyType,value);
				}
				
				setter.Invoke(theObject,new object[]{value});
			}
		}
		
		/// <summary>
		/// Tries to do an implicit adding of an item for lists or dictionaries.
		/// This is used when a property is defined as an object (for example) but it's actually a sequence
		/// </summary>
		/// <param name="theObject">The object which may be a list or sequence</param>
		/// <param name="propertyName">The property. If it's "Item" we'll add the node to the container</param>
		/// <param name="propertyNode">The node that may be an item to add</param>
		/// <returns></returns>
		private bool TryImplicitSequenceAdd(object theObject, string propertyName, XmlNode propertyNode)
		{
			bool processed=false;
			
			if(propertyNode.NodeType==XmlNodeType.Element && propertyName=="Item")
			{
				Type type=theObject.GetType();
				if(IsList(type))
				{
					ProcessList(theObject,propertyNode);
					processed=true;
				}
				else if(IsDictionary(type))
				{
					ProcessDictionary(theObject,propertyNode);
					processed=true;
				}
				else if(IsGenericCollection(type))
				{
					ProcessGenericCollection(theObject,propertyNode);
					processed=true;
				}
			}
			
			return processed;
		}
		
		/// <summary>
		/// Calls a method on the object. If the object is null then a call is made to a static method on the type
		/// </summary>
		/// <param name="type">The type of the object or the type to call against</param>
		/// <param name="theObject">The object to call. If null then a call is to a static method on "type"</param>
		/// <param name="callNode">A node whose name is the name of the method to call</param>
		internal void HandleCall(Type type, object theObject, XmlNode callNode)
		{
			XmlElement element=callNode as XmlElement;
			if(element==null) return; // NOTE: Early return
			
			string methodName=element.Name;
			
			List<Type> types=new List<Type>();
			List<object> parameters=new List<object>();
			
			foreach(XmlNode parameterNode in callNode.SelectNodes("*"))
			{
				object parameter=CreateObject(parameterNode,DefaultNodeType);
				types.Add(parameter.GetType());
				parameters.Add(parameter);
			}
			
			try
			{
				BindingFlags flags=BindingFlags.InvokeMethod;
				
				if(theObject==null)
				{
					// Assume it's a static method
					type.InvokeMember(methodName,MethodStaticBindings|flags,null,theObject,parameters.ToArray());
				}
				else
				{
					type.InvokeMember(methodName,MethodInstanceBindings|flags,null,theObject,parameters.ToArray());
				}
			}
			catch(Exception e)
			{
				// We'll get here if the member can't be found
				throw new XmlCreationException(e.Message);
			}
		}
		
		/// <summary>
		/// Returns an array of constructor parameters for the rootType
		/// </summary>
		/// <param name="rootType">The type of object we are creating</param>
		/// <param name="ctorNode">The constructor node whose children will be used as constructor parameters</param>
		private object[] GetConstructorParameters(Type rootType, XmlNode ctorNode)
		{
			if(ctorNode==null)
			{
				// It's a call to the default constructor
				return null;
			}
			
			// Loop over each child creating the types
			List<object> parameters=new List<object>();
			
			foreach(XmlNode parameterNode in ctorNode.SelectNodes("*"))
			{
				object o=CreateObject(parameterNode,DefaultNodeType);
				parameters.Add(o);
			}
			
			return parameters.ToArray();
		}
		
		private string ExpandText(string text)
		{
			text=TokenExpander.ExpandText(text,BeginToken,EndToken,m_UnknownVariableLookup);
			return text;
		}
	}
}
