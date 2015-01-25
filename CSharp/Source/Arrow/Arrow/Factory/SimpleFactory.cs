using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Arrow.Factory
{
	/// <summary>
	/// Provides a simple type factory that should be good enough for most needs.
	/// A type specific factory can delegate post of its operations to this class.
	/// This class is threadsafe
	/// </summary>
	/// <typeparam name="T">The type that all registered types must derive from or implement</typeparam>
	public class SimpleFactory<T>
	{
		private readonly object m_SyncRoot=new object();
		
		private Dictionary<string,Type> m_Types;
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		public SimpleFactory()
		{
			m_Types=new Dictionary<string,Type>();
		}
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="comparer">The comparer to use for resolving names</param>
		public SimpleFactory(IEqualityComparer<string> comparer)
		{
			m_Types=new Dictionary<string,Type>(comparer);
		}
	
		/// <summary>
		/// Registers a new type. If the name already exists its entry is overwritten
		/// </summary>
		/// <param name="name">The name to register the type under</param>
		/// <param name="type">The type to register</param>
		/// <exception cref="System.ArgumentNullException">name is null</exception>
		/// <exception cref="System.ArgumentNullException">type is null</exception>
		public void Register(string name, Type type)
		{
			if(name==null) throw new ArgumentNullException("name");
			if(type==null) throw new ArgumentNullException("type");
			
			if(typeof(T).IsAssignableFrom(type)==false)
			{
				throw new InvalidOperationException(name+" is not assignable to "+typeof(T).ToString());
			}
			
			lock(this.SyncRoot)
			{
				m_Types[name]=type;
			}
		}
		
		/// <summary>
		/// Creates an instance of the type registered with "name". The type must have a default constructor.
		/// </summary>
		/// <param name="name">The registered name of the type to create</param>
		/// <exception cref="System.ArgumentNullException">name is null</exception>
		public T Create(string name)
		{
			if(name==null) throw new ArgumentNullException("name");
			
			Type type=null;
			
			lock(this.SyncRoot)
			{
				m_Types.TryGetValue(name,out type);
			}
			
			if(type==null)
			{
				throw new ArrowException(name+" is not a registered type");
			}
			
			ConstructorInfo ctor=type.GetConstructor(Type.EmptyTypes);
			if(ctor==null) throw new ArrowException(name+" does not have a default constructor");
			
			return (T)ctor.Invoke(null);
		}
		
		/// <summary>
		/// Attempts to create an instance of the type registered with "name". The type must have a default constructor.
		/// </summary>
		/// <param name="name">The registered name of the type to try and create</param>
		/// <exception cref="System.ArgumentNullException">name is null</exception>
		/// <returns>An instance of the registered type, or the default value for T if the object could not be created</returns>
		public T TryCreate(string name)
		{
			if(name==null)  throw new ArgumentNullException("name");;
			
			Type type=null;
			
			lock(this)
			{
				m_Types.TryGetValue(name,out type);
			}
			
			if(type==null) return default(T);
			
			ConstructorInfo ctor=type.GetConstructor(Type.EmptyTypes);
			if(ctor==null) return default(T);
			
			return (T)ctor.Invoke(null);
		}
		
		/// <summary>
		/// Tries to get a named type
		/// </summary>
		/// <param name="name">The registered name to get</param>
		/// <param name="type">On exit is set to the type of the registered name, or null if the type could not be found</param>
		/// <returns>true if the type is found, otherwise false</returns>
		public bool TryGetType(string name, out Type type)
		{
			lock(this.SyncRoot)
			{
				return m_Types.TryGetValue(name,out type);
			}
		}
		
		/// <summary>
		/// Returns all the registered names
		/// </summary>
		/// <value>A list containing all the registered names</value>
		public List<string> Names
		{
			get
			{
				lock(this.SyncRoot)
				{
					return new List<string>(m_Types.Keys);
				}
			}
		}
		
		/// <summary>
		/// The object to syncronize on
		/// </summary>
		/// <value>An object to synchronize access</value>
		public object SyncRoot
		{
			get{return m_SyncRoot;}
		}
	}
}
