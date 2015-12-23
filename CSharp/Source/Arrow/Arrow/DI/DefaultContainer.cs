using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Arrow.DI
{
	/// <summary>
	/// A default implementation of a dependency injection container
	/// If an attempt is made to resolve a type that has not been registered then
	/// a transient instance of the type will be created
	/// </summary>
	public partial class DefaultContainer : IDIContainerRegister
	{
		private readonly object m_SyncRoot=new object();
		private readonly Dictionary<Type,Func<CreationContext,object>> m_Items=new Dictionary<Type,Func<CreationContext,object>>();

		private readonly DefaultContainer m_Parent;

		/// <summary>
		/// Initializes the instance
		/// </summary>
		public DefaultContainer() : this(null)
		{
		}

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="parent">The parent container, if applicable</param>
		internal DefaultContainer(DefaultContainer parent)
		{
			m_Parent=parent;
		}

		/// <summary>
		/// Resolves a type to an underlying implementation
		/// </summary>
		/// <param name="type">The type to resolve</param>
		/// <returns>An instance of the type</returns>
		public object Resolve(Type type)
		{
			if(type==null) throw new ArgumentNullException("type");

			var context=new CreationContext(this);
			
			var instance=Resolve(context,type);
			return instance;
		}

		/// <summary>
		/// Creates a new container scope.
		/// When resolving against the new scope the container will defer to its parent
		/// if it cannot resolve a type
		/// </summary>
		/// <returns>A new container scope</returns>
		public IDIContainerRegister NewScope()
		{
			return new DefaultContainer(this);
		}

		/// <summary>
		/// Register an existing instance as a singleton
		/// </summary>
		/// <typeparam name="T">The item to register</typeparam>
		/// <param name="exposedTypes">The types to expose from the item</param>
		/// <param name="item">The item to register</param>
		public IDIContainerRegister RegisterInstance<T>(IList<Type> exposedTypes, T item) where T:class
		{
			if(exposedTypes==null) throw new ArgumentNullException("exposedTypes");
			if(exposedTypes.Count==0) throw new ArgumentException("no items in exposedTypes");
			if(item==null) throw new ArgumentNullException("item");

			Type concreteType=typeof(T);
			EnsureTypeCompatible(exposedTypes,concreteType);

			lock(m_SyncRoot)
			{
				foreach(var type in exposedTypes)
				{
					Func<CreationContext,object> lookup=context=>item;
					m_Items.Add(type,lookup);
				}
			}

			return this;
		}
		

		/// <summary>
		/// Registers a concrete type against multiple exposed types
		/// </summary>
		/// <param name="exposedTypes">The type to expose</param>
		/// <param name="concreteType">A concreate implementation of the exposed type</param>
		/// <param name="lifetime">The lifetime of the type</param>
		public IDIContainerRegister Register(IList<Type> exposedTypes, Type concreteType, Lifetime lifetime)
		{
			if(exposedTypes==null) throw new ArgumentNullException("exposedTypes");
			if(exposedTypes.Count==0) throw new ArgumentException("no items in exposedTypes");
			if(concreteType==null) throw new ArgumentNullException("contreteType");

			// Do the sanity checks
			EnsureTypeCompatible(exposedTypes,concreteType);
			EnsureTypeConstraints(concreteType);

			// This will hold the singleton instance, if required
			// It's declared outside the foreach loop as all exposed types must use the same instance
			object instance=null;

			lock(m_SyncRoot)
			{
				foreach(var exposedType in exposedTypes)
				{
					Func<CreationContext,object> lookup=null;
			
					if(lifetime==Lifetime.Transient)
					{
						lookup=context=>
						{
							using(context.Scope(concreteType))
							{
								return CreateType(context,concreteType);
							}
						};
					}
					else
					{
						// This will give us a unique lock per singleton
						object singletonLock=new object();
					
						lookup=context=>
						{
							if(instance==null) 
							{
								lock(singletonLock)
								{
									if(instance==null) 
									{
										using(context.Scope(concreteType))
										{
											instance=CreateType(context,concreteType);
										}
									}
								}
							}

							return instance;
						};
					}

					m_Items.Add(exposedType,lookup);
				}
			}

			return this;
		}

		/// <summary>
		/// Checks to make sure none of the specified types are already registered
		/// By calling this we stop registration over an existing type, which may or may not be desireable
		/// </summary>
		/// <param name="types"></param>
		private void EnsureTypeNotRegistered(IList<Type> types)
		{
			foreach(var type in types)
			{
				if(m_Items.ContainsKey(type)) throw new ArgumentException("type already registered: "+type.ToString());
			}
		}

		/// <summary>
		/// Checks to make sure that the type follows are rules
		/// </summary>
		/// <param name="type"></param>
		private void EnsureTypeConstraints(Type type)
		{
			if(type.IsClass==false) throw new ContainerException("T must be a class");
			if(type.IsAbstract) throw new ContainerException("T is abstract");
		}

		/// <summary>
		/// Checks that the concrete type does implement all the specified base types
		/// </summary>
		/// <param name="baseTypes"></param>
		/// <param name="concreteType"></param>
		private void EnsureTypeCompatible(IList<Type> baseTypes, Type concreteType)
		{
			foreach(var baseType in baseTypes)
			{
				if(baseType.IsAssignableFrom(concreteType)==false) 
				{
					string message=string.Format("{0} cannot be converted to {1}",concreteType.Name,baseType.Name);
					throw new ArgumentException(message);
				}
			}
		}

		/// <summary>
		/// Create an instance of a type
		/// </summary>
		/// <param name="context"></param>
		/// <param name="typeInfo"></param>
		/// <returns></returns>
		private object CreateType(CreationContext context, Type typeInfo)
		{
			var constructor=SelectConstructor(typeInfo);			
			var parameterInfo=constructor.GetParameters();	

			object[] parameters=new object[parameterInfo.Length];

			for(int i=0; i<parameterInfo.Length; i++)
			{
				Type type=parameterInfo[i].ParameterType;
				
				// NOTE: We need to use the StartContainer to pick up any overrides in child containers
				object value=context.StartContainer.Resolve(context,type);

				parameters[i]=value;
			}

			object instance=constructor.Invoke(parameters);
			return instance;
		}

		/// <summary>
		/// Resolves the specified type to an instance
		/// </summary>
		/// <param name="context"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		private object Resolve(CreationContext context, Type type)
		{
			// First, see if we've got an explicit lookup function for the type
			lock(m_SyncRoot)
			{
				Func<CreationContext,object> lookup;
				if(m_Items.TryGetValue(type,out lookup)) 
				{
					return lookup(context);
				}
			}

			// If there's a parent then ask them to try
			if(m_Parent!=null)
			{
				return m_Parent.Resolve(context,type);
			}

			using(context.Scope(type))
			{
				// The type isn't registered, so assume we want to create a transient instance of a concrete type
				EnsureTypeConstraints(type);

				object instance=CreateType(context,type);
				return instance;
			}
		}

		/// <summary>
		/// Determines which constructor to call when creating an instance
		/// </summary>
		/// <param name="typeInfo"></param>
		/// <returns></returns>
		private ConstructorInfo SelectConstructor(Type typeInfo)
		{
			ConstructorInfo candidate=null;
			bool ambigious=false;

			foreach(var ctor in typeInfo.GetConstructors())
			{
				if(ctor.IsPublic==false) continue;

				if(candidate==null)
				{
					candidate=ctor;
				}
				else
				{
					// We'll prefer the constructor with the most parameters
					if(ctor.GetParameters().Length>candidate.GetParameters().Length)
					{
						candidate=ctor;
						ambigious=false;
					}
					else if(ctor.GetParameters().Length==candidate.GetParameters().Length)
					{
						ambigious=true;
					}
				}
			}

			if(ambigious) throw new ContainerException("Two or more constructors have the same arity: "+typeInfo.ToString());
			if(candidate==null) throw new ContainerException("Could not find a public constructor: "+typeInfo.ToString());

			return candidate;
		}

	}
}
