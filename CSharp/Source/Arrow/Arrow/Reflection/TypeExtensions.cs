using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Arrow.Reflection
{
    /// <summary>
    /// Type extension methods
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Determines if a type is a delegate
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>true if the type is a delegate, otherwise false</returns>
        public static bool IsDelegate(this Type type)
        {
            if(type == null) throw new ArgumentNullException("type");

            return typeof(Delegate).IsAssignableFrom(type);
        }

        /// <summary>
        /// Determines if a type represents something that can set to null
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>true if the type can be assigned to null, false otherwise</returns>
        public static bool CanBeSetToNull(this Type type)
        {
            if(type.IsValueType == false) return true;
            if(Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T>
            return false; // value-type
        }

        /// <summary>
        /// Returns the name of the default member for a type.
        /// In C# this is typically the parameter called "this"
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>The name of the default member, or null if the type does not have one</returns>
        public static string? DefaultMemberName(this Type type)
        {
            if(type == null) throw new ArgumentNullException("type");

            string? name = null;

            var attributes = type.CustomAttributes<DefaultMemberAttribute>(true);
            if(attributes != null && attributes.Length > 0)
            {
                name = attributes[0].MemberName;
            }

            return name;
        }

        /// <summary>
        /// Returns all the base types and interfaces available against a type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetBasesAndInterfaces(this Type type)
        {
            if(type == null) throw new ArgumentNullException("type");

            HashSet<Type> all = new HashSet<Type>();

            Stack<Type> types = new Stack<Type>(13);
            types.Push(type);

            while(types.Count != 0)
            {
                Type next = types.Pop();
                all.Add(next);

                var baseType = next.BaseType;
                if(baseType != null) types.Push(baseType);

                foreach(var @interface in next.GetInterfaces())
                {
                    if(all.Contains(@interface) == false) types.Push(@interface);
                }
            }

            return all;
        }

        /// <summary>
        /// Searches for a property against a type and all interfaces it implements
        /// </summary>
        /// <param name="type">The type to start searching from</param>
        /// <param name="propertyName">The name of the property to search for</param>
        /// <param name="bindingFlags">How to lookup the property against a type</param>
        /// <returns>The PropertyInfo instance for the property, or null if the property could not be found</returns>
        public static PropertyInfo? GetPropertyAndSearchInterfaces(this Type type, string propertyName, BindingFlags bindingFlags)
        {
            if(type == null) throw new ArgumentNullException("type");

            if(type.IsInterface == false)
            {
                return type.GetProperty(propertyName, bindingFlags);
            }

            Stack<Type> types = new Stack<Type>(13);
            types.Push(type);

            while(types.Count != 0)
            {
                Type next = types.Pop();
                var property = next.GetProperty(propertyName, bindingFlags);
                if(property != null) return property;

                foreach(var @interface in next.GetInterfaces())
                {
                    types.Push(@interface);
                }
            }

            return null;
        }

        /// <summary>
        /// Get the method who parameter types exactly match those specified
        /// </summary>
        /// <param name="type">The type to query</param>
        /// <param name="methodName">The name of the method to get</param>
        /// <param name="bindingFlags">The binding flags for the lookup</param>
        /// <param name="parameters">The types of the parameters for the method</param>
        /// <returns>The method, if found, otherwise null</returns>
        public static MethodInfo? GetExactMethod(this Type type, string methodName, BindingFlags bindingFlags, params Type[] parameters)
        {
            if(type == null) throw new ArgumentNullException("type");

            var method = type.GetMethod
            (
                methodName,
                bindingFlags | BindingFlags.ExactBinding,
                null,
                parameters,
                null
            );

            return method;
        }
    }
}
