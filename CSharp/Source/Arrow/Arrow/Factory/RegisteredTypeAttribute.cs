using System;
using System.Collections.Generic;
using System.Text;

namespace Arrow.Factory
{
	/// <summary>
	/// Registers a type with a factory.
	/// The factory must have a public static method called Register(string name, Type type)
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited=false, AllowMultiple=true)]
	public class RegisteredTypeAttribute : Attribute
	{
		private Type m_FactoryType;
		private string m_Name;
		
		/// <summary>
		/// Initializes an instance
		/// </summary>
		/// <param name="factoryType">Tye type of factory to register with</param>
		/// <param name="name">The name to register in the factory</param>
		/// <exception cref="System.ArgumentNullException">factoryType is null</exception>
		/// <exception cref="System.ArgumentNullException">name is null</exception>
		public RegisteredTypeAttribute(Type factoryType, string name)
		{
			if(factoryType==null) throw new ArgumentNullException("factoryType");
			if(name==null) throw new ArgumentNullException("name");
		
			m_FactoryType=factoryType;
			m_Name=name;
		}
		
		/// <summary>
		/// Returns the type of the factory that will manufacture the type
		/// </summary>
		/// <value>The type of the factory</value>
		public Type FactoryType
		{
			get{return m_FactoryType;}
		}
		
		/// <summary>
		/// Returns the name that the type should be registered under
		/// </summary>
		/// <value>The name to register</value>
		public string Name
		{
			get{return m_Name;}
		}
	}
}
