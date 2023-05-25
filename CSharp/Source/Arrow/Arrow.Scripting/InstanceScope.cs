using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;

using Arrow.Collections;
using Arrow.Reflection;

namespace Arrow.Scripting
{
	/// <summary>
	/// Allows you to treat an instance variable as a source of values.
	/// Public properies and fields can be accessed by name and will
	/// be resolved against a specified instance.
	/// 
	/// Lambdas are generated and cached for a type to make lookup as quick as possible
	/// </summary>
	/// <typeparam name="T">The type that will be be looked up against</typeparam>
	public class InstanceScope<T> : IVariableRead
	{
		private static readonly BindingFlags BindFlags=BindingFlags.Public|BindingFlags.Instance;

        private readonly static Dictionary<string, Func<T, object?>> s_LookupCache = new();
        private readonly static Dictionary<string, Func<T, object?>> s_InsensitveLookupCache = new(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, Func<T, object?>> m_Lookup;
        private readonly T m_Instance;

		static InstanceScope()
		{
			GenerateCache();
		}

		/// <summary>
		/// Initializes the instance for case insensitve lookup
		/// </summary>
		/// <param name="instance">The instance variable to look up against</param>
		public InstanceScope(T instance) : this(instance,CaseMode.Insensitive)
		{
		}

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="instance">The instance variable to look up against</param>
        /// <param name="caseMode">The case mode to use for resolving names</param>
        public InstanceScope(T instance, CaseMode caseMode)
        {
            if(instance == null) throw new ArgumentNullException("instance");
            m_Instance = instance;
            m_Lookup = (caseMode == CaseMode.Insensitive ? s_InsensitveLookupCache : s_LookupCache);
        }

        /// <summary>
        /// Returns the value of a variable
        /// </summary>
        /// <param name="variableName">The name of the variable to lookup</param>
        /// <returns>The value of the variable</returns>
        /// <exception cref="System.ArgumentNullException">if variableName is null</exception>
        /// <exception cref="Arrow.Scripting.VariableNotFoundException">if the variable is not defined</exception>
        public object? GetVariable(string variableName)
		{
            if(variableName == null) throw new ArgumentNullException("variableName");

            if(m_Lookup.TryGetValue(variableName, out var lookup) == false)
            {
                throw new VariableNotFoundException(variableName);
            }

            return lookup(m_Instance);
		}

        /// <summary>
        /// Attempts to get the value of a variable
        /// </summary>
        /// <param name="variableName">The name of the variable to lookup</param>
        /// <param name="result">On success the value of the variable</param>
        /// <returns>true on success, false if the variable could not be found</returns>
        /// <exception cref="System.ArgumentNullException">if variableName is null</exception>
        public bool TryGetVariable(string variableName, out object? result)
        {
            if(variableName == null) throw new ArgumentNullException("variableName");

            bool found = false;

            if(m_Lookup.TryGetValue(variableName, out var lookup))
            {
                result = lookup(m_Instance);
                found = true;
            }
            else
            {
                result = default(T);
                found = false;
            }

            return found;
        }

        /// <summary>
        /// Checks to see if a variable is present
        /// </summary>
        /// <param name="variableName">The name of the variable to lookup</param>
        /// <returns>true if the variable is present, otherwise false</returns>
        /// <exception cref="System.ArgumentNullException">if variableName is null</exception>
        public bool IsDeclared(string variableName)
        {
            if(variableName == null) throw new ArgumentNullException("variableName");
            return m_Lookup.ContainsKey(variableName);
        }

        private static void GenerateCache()
        {
            // First grab the properties
            foreach(var property in typeof(T).GetProperties(BindFlags))
            {
                // Ignore unreadable properties
                if(property.CanRead == false) continue;

                // Ignore properties that take parameters (this[,,,])
                if(property.GetGetMethod()!.GetParameters().Length != 0) continue;

                var parameter = Expression.Parameter(typeof(T));
                var propertyRead = Expression.Property(parameter, property);

                var lambda = Expression.Lambda<Func<T, object>>(Expression.Convert(propertyRead, typeof(object)), parameter).Compile();

                s_LookupCache.Add(property.Name, lambda);
            }

            // Now the fields
            foreach(var field in typeof(T).GetFields(BindFlags))
            {
                var parameter = Expression.Parameter(typeof(T));
                var fieldValue = Expression.Field(parameter, field.Name);
                var lambda = Expression.Lambda<Func<T, object>>(Expression.Convert(fieldValue, typeof(object)), parameter).Compile();

                s_LookupCache.Add(field.Name, lambda);
            }

            // Finally generate the case insensitive version
            foreach(var pair in s_LookupCache)
            {
                if(s_InsensitveLookupCache.ContainsKey(pair.Key) == false)
                {
                    s_InsensitveLookupCache.Add(pair.Key, pair.Value);
                }
            }
        }
    }

    /// <summary>
    /// Useful factory methods for instance scopes
    /// </summary>
    public static class InstanceScope
    {
        /// <summary>
        /// Creates an InstanceScope instance
        /// </summary>
        /// <typeparam name="T">The type that will be be looked up against</typeparam>
        /// <param name="instance">The instance to lookup against</param>
        /// <returns>An InstanceScope instance</returns>
        public static InstanceScope<T> Create<T>(T instance)
        {
            return new InstanceScope<T>(instance);
        }

        /// <summary>
        /// Creates an InstanceScope instance
        /// </summary>
        /// <typeparam name="T">The type that will be be looked up against</typeparam>
        /// <param name="instance">The instance to lookup against</param>
        /// <param name="caseMode">The case mode to use for resolving names</param>
        /// <returns>An InstanceScope instance</returns>
        public static InstanceScope<T> Create<T>(T instance, CaseMode caseMode)
        {
            return new InstanceScope<T>(instance, caseMode);
        }
    }
}
