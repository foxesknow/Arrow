using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;

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
        private readonly object m_SyncRoot = new object();

        private Dictionary<string, TypeInfo> m_Types;

        /// <summary>
        /// Initializes the instance
        /// </summary>
        public SimpleFactory()
        {
            m_Types = new Dictionary<string, TypeInfo>();
        }

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="comparer">The comparer to use for resolving names</param>
        public SimpleFactory(IEqualityComparer<string> comparer)
        {
            m_Types = new Dictionary<string, TypeInfo>(comparer);
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
            if(name == null) throw new ArgumentNullException("name");
            if(type == null) throw new ArgumentNullException("type");

            if(typeof(T).IsAssignableFrom(type) == false)
            {
                throw new InvalidOperationException(name + " is not assignable to " + typeof(T).ToString());
            }

            var factory = Compile(name, type);

            lock(this.SyncRoot)
            {
                m_Types[name] = new TypeInfo() { Type = type, Factory = factory };
            }
        }

        /// <summary>
        /// Creates an instance of the type registered with "name". The type must have a default constructor.
        /// </summary>
        /// <param name="name">The registered name of the type to create</param>
        /// <exception cref="System.ArgumentNullException">name is null</exception>
        public T Create(string name)
        {
            if(name == null) throw new ArgumentNullException("name");

            TypeInfo? typeInfo = null;

            lock(this.SyncRoot)
            {
                m_Types.TryGetValue(name, out typeInfo);
            }

            if(typeInfo == null)
            {
                throw new ArrowException(name + " is not a registered type");
            }

            return (T)typeInfo.Factory!();
        }

        /// <summary>
        /// Attempts to create an instance of the type registered with "name". The type must have a default constructor.
        /// </summary>
        /// <param name="name">The registered name of the type to try and create</param>
        /// <exception cref="System.ArgumentNullException">name is null</exception>
        /// <returns>An instance of the registered type, or the default value for T if the object could not be created</returns>
        public T TryCreate(string name)
        {
            if(name == null) throw new ArgumentNullException("name");

            TypeInfo? typeInfo = null;

            lock(this.SyncRoot)
            {
                m_Types.TryGetValue(name, out typeInfo);
            }

            if(typeInfo == null) return default!;

            return (T)typeInfo.Factory!();
        }

        /// <summary>
        /// Tries to get a named type
        /// </summary>
        /// <param name="name">The registered name to get</param>
        /// <param name="type">On exit is set to the type of the registered name, or null if the type could not be found</param>
        /// <returns>true if the type is found, otherwise false</returns>
        public bool TryGetType(string name, out Type? type)
        {
            TypeInfo? typeInfo = null;

            lock(this.SyncRoot)
            {

                m_Types.TryGetValue(name, out typeInfo);
            }

            type = (typeInfo == null ? null : typeInfo.Type);
            return typeInfo != null;
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

        /// <summary>
        /// Creates a function that will create an instance of the specified type.
        /// This is to avoid a reflectio call to the constructor for each invocation.
        /// </summary>
        private Func<object> Compile(string name, Type type)
        {
            var ctor = type.GetConstructor(Type.EmptyTypes);
            if(ctor == null) throw new ArrowException(name + " does not have a default constructor");

            var newExpression = Expression.New(ctor);

            Expression<Func<object>>? function = null;

            if(type.IsValueType)
            {
                // We'll need to box it
                function = Expression.Lambda<Func<object>>(Expression.Convert(newExpression, typeof(object)));
            }
            else
            {
                function = Expression.Lambda<Func<object>>(newExpression);
            }

            return function.Compile();
        }

        class TypeInfo
        {
            public Type? Type;
            public Func<object>? Factory;
        }
    }
}
