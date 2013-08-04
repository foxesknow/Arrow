using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Scripting;
using Arrow.Collections;

namespace Arrow.Dynamic
{
	/// <summary>
	/// Implements a type cache, mapping the name of a type to its type object
	/// </summary>
	public class TypeCache
	{
		private readonly Dictionary<string,Type> m_Types;

		/// <summary>
		/// Initializes the instance in cse-insensitive mode
		/// </summary>
		public TypeCache() : this(CaseMode.Insensitive)
		{
		}

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="caseMode">The case mode to use for the type names</param>
		public TypeCache(CaseMode caseMode)
		{
			IEqualityComparer<string> comparer=null;
			if(caseMode==CaseMode.Insensitive) comparer=IgnoreCaseEqualityComparer.Instance;

			m_Types=new Dictionary<string,Type>(comparer);
		}

		/// <summary>
		/// Adds a type to the cache
		/// </summary>
		/// <param name="name">The name of the type</param>
		/// <param name="type">The actual type</param>
		public void Add(string name, Type type)
		{
			if(name==null) throw new ArgumentNullException("name");
			if(type==null) throw new ArgumentNullException("type");

			m_Types[name]=type;
		}

		/// <summary>
		/// Attempts to get a type
		/// </summary>
		/// <param name="name">The name of the type</param>
		/// <param name="type">On success, set to the actual type</param>
		/// <returns>true if the type was found, otherwise false</returns>
		public bool TryGetType(string name, out Type type)
		{
			if(name==null) throw new ArgumentNullException("name");

			return m_Types.TryGetValue(name,out type);
		}

		/// <summary>
		/// Checks to see if a type is present
		/// </summary>
		/// <param name="name">The name of the type</param>
		/// <returns>true if the type is present, otherwise false</returns>
		public bool Contains(string name)
		{
			if(name==null) throw new ArgumentNullException("name");

			return m_Types.ContainsKey(name);
		}

		/// <summary>
		/// Register the CLS compatible types with their C# names
		/// </summary>
		public void RegisterCLS()
		{
			m_Types["object"]=typeof(object);
			m_Types["byte"]=typeof(byte);
			m_Types["bool"]=typeof(bool);
			m_Types["char"]=typeof(char);
			m_Types["short"]=typeof(short);
			m_Types["int"]=typeof(int);
			m_Types["long"]=typeof(long);
			m_Types["float"]=typeof(float);
			m_Types["double"]=typeof(double);
			m_Types["decimal"]=typeof(decimal);
			m_Types["string"]=typeof(string);
		}

		/// <summary>
		/// Registers the non cls-compatible types with their C# names
		/// </summary>
		public void RegisterNonCLS()
		{
			m_Types["sbyte"]=typeof(sbyte);
			m_Types["ushort"]=typeof(ushort);
			m_Types["uint"]=typeof(uint);
			m_Types["ulong"]=typeof(ulong);
		}
	}
}
