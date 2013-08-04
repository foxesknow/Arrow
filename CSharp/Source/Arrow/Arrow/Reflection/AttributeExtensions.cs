using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Arrow.Reflection
{
	/// <summary>
	/// Attribute extension methods
	/// </summary>
	public static class AttributeExtensions
	{
		/// <summary>
		/// Gets custom attributes assigned to an assembly
		/// </summary>
		/// <typeparam name="T">The type of attribute required</typeparam>
		/// <param name="assembly">The assembly to query for attributes</param>
		/// <returns>An array of attributes. If nothing was found then the array will be empty</returns>
		public static T[] CustomAttributes<T>(this Assembly assembly) where T:Attribute
		{
			if(assembly==null) throw new ArgumentNullException("assembly");
			
			object[] attributes=assembly.GetCustomAttributes(typeof(T),true); // NOTE: the true is ignored
			return (T[])attributes;
		}
		
		/// <summary>
		/// Gets custom attributes assigned to a member
		/// </summary>
		/// <typeparam name="T">The type of attribute required</typeparam>
		/// <param name="memberInfo">The memeber to query for attributes</param>
		/// <param name="inherit">Specifies whether to search the member's inheritance chain to find the attributes</param>
		/// <returns>An array of attributes. If nothing was found then the array will be empty</returns>
		public static T[] CustomAttributes<T>(this MemberInfo memberInfo, bool inherit) where T:Attribute
		{
			if(memberInfo==null) throw new ArgumentNullException("memberInfo");
			
			object[] attributes=memberInfo.GetCustomAttributes(typeof(T),inherit);
			return (T[])attributes;
		}
		
		/// <summary>
		/// Gets custom attributes assigned to a parameter
		/// </summary>
		/// <typeparam name="T">The type of attribute required</typeparam>
		/// <param name="parameterInfo">The parameter to query for attributes</param>
		/// <returns>An array of attributes. If nothing was found then the array will be empty</returns>
		public static T[] CustomAttributes<T>(this ParameterInfo parameterInfo) where T:Attribute
		{
			if(parameterInfo==null) throw new ArgumentNullException("parameterInfo");
			
			object[] attributes=parameterInfo.GetCustomAttributes(typeof(T),true); // NOTE: the true is ignored
			return (T[])attributes;
		}
		
		/// <summary>
		/// Gets custom attributes assigned to a module
		/// </summary>
		/// <typeparam name="T">The type of attribute required</typeparam>
		/// <param name="module">The module to query for attributes</param>
		/// <returns>An array of attributes. If nothing was found then the array will be empty</returns>
		public static T[] CustomAttributes<T>(this Module module) where T:Attribute
		{
			if(module==null) throw new ArgumentNullException("module");
			
			object[] attributes=module.GetCustomAttributes(typeof(T),true); // NOTE: the true is ignored
			return (T[])attributes;
		}

	}
}
