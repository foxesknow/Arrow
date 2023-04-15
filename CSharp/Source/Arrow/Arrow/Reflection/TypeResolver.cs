using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Reflection;

using Arrow.Collections;

namespace Arrow.Reflection
{
	/// <summary>
	/// Provides a collection of routines for resolving types
	/// </summary>
	public static class TypeResolver
	{
        private static Dictionary<string, Type> s_TypeAlias = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        static TypeResolver()
        {
            s_TypeAlias.Add("bool", typeof(bool));
            s_TypeAlias.Add("char", typeof(char));
            s_TypeAlias.Add("byte", typeof(byte));
            s_TypeAlias.Add("short", typeof(short));
            s_TypeAlias.Add("int", typeof(int));
            s_TypeAlias.Add("long", typeof(long));
            s_TypeAlias.Add("float", typeof(float));
            s_TypeAlias.Add("double", typeof(double));
            s_TypeAlias.Add("decimal", typeof(decimal));
            s_TypeAlias.Add("sbyte", typeof(sbyte));
            s_TypeAlias.Add("ushort", typeof(ushort));
            s_TypeAlias.Add("uint", typeof(uint));
            s_TypeAlias.Add("ulong", typeof(ulong));
            s_TypeAlias.Add("string", typeof(string));
        }

        /// <summary>
        /// Attempts to coerce "data" to the specified type
        /// </summary>
        /// <param name="type">The type to coerce to</param>
        /// <param name="data">The data to coerce</param>
        /// <returns>"data" coerced to "type"</returns>
        /// <exception cref="System.NotSupportedException">The data could not coerced to the required type</exception>
        /// <exception cref="System.ArgumentException">The type specifed an enum and "data" is not valid for the enumeration</exception>
        /// <remarks>
        /// <para>
        /// If the type is an enumeration that can be treated as a bit field then
        /// data may contain a string of values seperated by |
        /// </para>
        /// </remarks>
        public static object? CoerceToType(Type type, object? data)
        {
            if(data == null)
            {
                if(type.IsValueType)
                {
                    // If it's a nullable type then null is allowed
                    if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return null;
                    }
                    else
                    {
                        throw new NotSupportedException("cannot convert null to a value type");
                    }
                }
                return null;
            }
            if(type == data.GetType()) return data;

            if(type.IsEnum)
            {
                object value = ExpandEnum(type, data.ToString()!);
                return value; // NOTE: Early return
            }

            if(type.IsAssignableFrom(data.GetType()))
            {
                return data;
            }

            object? parameter;

            // Use the type converter framework to convert the value.
            // If there isn't a converter to do the convesion then fall back to the Convert class
            TypeConverter typeConverter = TypeDescriptor.GetConverter(type);
            if(typeConverter != null && typeConverter.CanConvertFrom(data.GetType()))
            {
                parameter = typeConverter.ConvertFrom(data);
            }
            else
            {
                // The destination type can't do the conversion, so see if 
                // there's a converter on the data that can convert to the type
                typeConverter = TypeDescriptor.GetConverter(data.GetType());
                if(typeConverter != null && typeConverter.CanConvertTo(type))
                {
                    parameter = typeConverter.ConvertTo(data, type);
                }
                else
                {
                    parameter = Convert.ChangeType(data, type);
                }
            }

            return parameter;
        }

        /// <summary>
        /// Expands an enum encoded as a string
        /// </summary>
        /// <param name="type">The type of the enumeration</param>
        /// <param name="data">A string representation of an enumeration value</param>
        /// <exception cref="System.ArgumentException">"data" is not valid for the enumeration</exception>
        /// <remarks>
        /// If the type if a flag enum then the 
        /// string can contain | or comma (,) to indicate or'ing values together
        /// </remarks>
        public static object ExpandEnum(Type type, string data)
        {
            bool isFlag = type.IsDefined(typeof(FlagsAttribute), true);

            // The type isn't a flag, so the data must be the value
            if(isFlag == false)
            {
                return Enum.Parse(type, data, true);
            }

            // For flag enums we'll allow for ORing the values
            ulong value = 0;
            string[] parts = data.Split('|', ',');

            foreach(string part in parts)
            {
                object enumValue = Enum.Parse(type, part.Trim(), true);
                value |= Convert.ToUInt64(enumValue);
            }

            object flagValue = Enum.ToObject(type, value);
            return flagValue;
        }

        /// <summary>
        /// Retrieves the type in the format typename[,assembly].
        /// NOTE: This method does not support the clr-namespace format.
        /// </summary>
        /// <param name="typeName">The name of the type and an optional assembly that indicates where the type lives</param>
        /// <returns>The type represented by the  type name</returns>
        public static Type GetEncodedType(string typeName)
        {
            string[] parts = typeName.Split(new char[]{','}, 2);

            string name = parts[0];
            string? assemblyName = (parts.Length == 2 ? parts[1] : null);

            Type type = GetEncodedType(name, assemblyName);

            return type;
        }

        /// <summary>
        /// Retrieves a type for a class in the (optional) assembly
        /// NOTE: This method does not support the clr-namespace format.
        /// </summary>
        /// <param name="typeName">The name of the type to fetch</param>
        /// <param name="assemblyName">The assembly that contains the type</param>
        /// <returns>The requested type</returns>
        public static Type GetEncodedType(string typeName, string? assemblyName)
        {
            Type? type = null;
            if(s_TypeAlias.TryGetValue(typeName, out type))
            {
                // There's an alias for the type
                return type;
            }

            // Do a general search...
            type = Type.GetType(typeName.Trim(), false, true);

            // Try and resolve using the assembly, if possible
            if(type == null && assemblyName != null)
            {
                string encodedName = string.Format("{0},{1}", typeName, assemblyName);
                type = Type.GetType(encodedName, false, true);
            }

            if(type == null && assemblyName != null)
            {
                Assembly assembly = LoadAssembly(assemblyName.Trim());

                if(assembly != null)
                {
                    type = assembly.GetType(typeName, true, true);
                }
            }

            if(type == null) throw new ArrowException("could not resolve type: " + typeName);

            return type;
        }

        /// <summary>
        /// Loads an assembly into memory by examining the name and working
        /// out if it's a fully versioned name, or a partial name
        /// </summary>
        /// <param name="assemblyName">The name of the assembly</param>
        /// <returns>An assembly</returns>
        public static Assembly LoadAssembly(string assemblyName)
        {
            if(assemblyName == null) throw new ArgumentNullException("assemblyName");

            Assembly? assembly = null;

            if(assemblyName.IndexOf(',') != -1)
            {
                // It's a fully versioned assembly
                assembly = Assembly.Load(assemblyName);
            }
            else
            {
                // LoadWithPartialName has been deprecated, so we'll disable the warning to avoid "poluting" the build output
#pragma warning disable 618
                assembly = Assembly.LoadWithPartialName(assemblyName);
#pragma warning restore 618
            }

            return assembly!;
        }
    }
}
