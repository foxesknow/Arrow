using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;

using Arrow.Reflection;

namespace Arrow.Dynamic
{
	public static partial class ExpressionEx
	{
		/// <summary>
		/// Looks for a property or field with the specified name
		/// </summary>
		/// <param name="type">The type to query</param>
		/// <param name="name">The name of the property or field</param>
		/// <param name="flags">Describes how to locate the property or field</param>
		/// <returns>An expression that provides access to the property or field</returns>
		public static MemberExpression PropertyOrField(Type type, string name, BindingFlags flags)
		{
			if(type==null) throw new ArgumentNullException("type");
			if(name==null) throw new ArgumentNullException("name");

			MemberExpression x=null;

			// It's a field or property access
			FieldInfo fieldInfo=type.GetField(name,flags);

			if(fieldInfo!=null)
			{
				x=Expression.Field(null,fieldInfo);
			}
			else
			{
				var property=type.GetProperty(name,flags);
				if(property==null) throw new DynamicException("not a property or field: "+name);					
				x=Expression.Property(null,property);
			}

			return x;
		}

		/// <summary>
		/// Loods for a property or field with the specified name
		/// </summary>
		/// <param name="type">The type to query</param>
		/// <param name="name">The name of the property or field</param>
		/// <param name="flags">Describes how to locate the property or field</param>
		/// <param name="expression">On success set to an expression that provides access to the property or field</param>
		/// <returns>True if a property or field was found, otherwise false</returns>
		public static bool TryPropertyOrField(Type type, string name, BindingFlags flags, out MemberExpression expression)
		{
			if(type==null) throw new ArgumentNullException("type");
			if(name==null) throw new ArgumentNullException("name");

			bool found=false;
			expression=null;

			// It's a field or property access
			FieldInfo fieldInfo=type.GetField(name,flags);

			if(fieldInfo!=null)
			{
				expression=Expression.Field(null,fieldInfo);
				found=true;
			}
			else
			{
				var property=type.GetProperty(name,flags);
				if(property!=null)
				{
					expression=Expression.Property(null,property);
					found=true;
				}
			}

			return found;
		}

		/// <summary>
		/// Looks for a property or field with the specified name
		/// </summary>
		/// <param name="instance">The instance to query for the property or field</param>
		/// <param name="name">The name of the property or field</param>
		/// <param name="flags">Describes how to locate the property or field</param>
		/// <returns>An expression that provides access to the property or field</returns>
		public static MemberExpression PropertyOrField(Expression instance, string name, BindingFlags flags)
		{
			if(instance==null) throw new ArgumentNullException("instance");
			if(name==null) throw new ArgumentNullException("name");

			Type type=instance.Type;
			MemberExpression x=null;

			// It's a field or property access
			FieldInfo fieldInfo=type.GetField(name,flags);

			if(fieldInfo!=null)
			{
				x=Expression.Field(instance,fieldInfo);
			}
			else
			{
				var property=type.GetPropertyAndSearchInterfaces(name,flags);
				if(property==null) throw new DynamicException("not a property or field: "+name);
				x=Expression.Property(instance,property);
			}

			return x;
		}

		/// <summary>
		/// Looks for a property or field with the specified name
		/// </summary>
		/// <param name="instance">The instance to query for the property or field</param>
		/// <param name="name">The name of the property or field</param>
		/// <param name="flags">Describes how to locate the property or field</param>
		/// <param name="expression">On success set to an expression that provides access to the property or field</param>
		/// <returns>True if a property or field was found, otherwise false</returns>
		public static bool TryPropertyOrField(Expression instance, string name, BindingFlags flags, out MemberExpression expression)
		{
			if(instance==null) throw new ArgumentNullException("instance");
			if(name==null) throw new ArgumentNullException("name");

			Type type=instance.Type;
			
			bool found=false;
			expression=null;

			// It's a field or property access
			FieldInfo fieldInfo=type.GetField(name,flags);

			if(fieldInfo!=null)
			{
				expression=Expression.Field(instance,fieldInfo);
				found=true;
			}
			else
			{
				var property=type.GetProperty(name,flags);
				if(property!=null) 
				{
					expression=Expression.Property(instance,property);
					found=true;
				}
			}

			return found;
		}
	}
}
