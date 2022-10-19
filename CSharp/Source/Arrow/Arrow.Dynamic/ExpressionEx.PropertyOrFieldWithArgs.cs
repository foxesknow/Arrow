using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;

using Arrow.Reflection;
using System.Diagnostics.CodeAnalysis;

namespace Arrow.Dynamic
{
	public static partial class ExpressionEx
	{
		public static bool TryArrayAccess(Expression instance, IEnumerable<Expression> indexes, [NotNullWhen(true)] out Expression? expression)
		{
			expression=null;

			if(instance.Type.IsArray)
			{
				expression=Expression.ArrayAccess(instance,indexes);
			}
			else
			{
				// Look for a default member
				string defaultMemberName=instance.Type.DefaultMemberName();
				if(defaultMemberName==null) return false;

				expression=Expression.Property(instance,defaultMemberName,indexes.ToArray());
			}

			return expression!=null;
		}

		public static bool TryPropertyOrFieldWithArguments(Expression instance, string name, BindingFlags bindingFlags, IEnumerable<Expression> indexes, [NotNullWhen(true)] out Expression? expression)
		{
			expression=null;

			var fieldInfo=instance.Type.GetField(name, bindingFlags);
			if(fieldInfo!=null)
			{
				var fieldInstance=Expression.Field(instance,fieldInfo);

				// If it's an array then all is well
				if(fieldInfo.FieldType.IsArray)
				{
					expression=Expression.ArrayAccess(fieldInstance,indexes);
					return true;
				}

				// It's a type, so we call the default member
				string defaultMemberName=fieldInfo.FieldType.DefaultMemberName();
				if(defaultMemberName==null) return false;

				name=defaultMemberName;
				instance=fieldInstance;
			}
			else
			{
				var propertyInfo=instance.Type.GetProperty(name, bindingFlags);
				if(propertyInfo==null) return false;

				var propertyInstance=Expression.Property(instance,propertyInfo);
				if (propertyInfo.PropertyType.IsArray)
				{
					expression=Expression.ArrayAccess(propertyInstance,indexes);
					return true;
				}

				// It's a type, so we call the default member
				string defaultMemberName=propertyInfo.PropertyType.DefaultMemberName();
				if(defaultMemberName==null) return false;

				name=defaultMemberName;
				instance=propertyInstance;
			}

			expression=Expression.Property(instance,name,indexes.ToArray());
			return true;
		}

		public static bool TryPropertyOrFieldWithArguments(Type type, string name, BindingFlags bindingFlags, IEnumerable<Expression> indexes, [NotNullWhen(true)] out Expression? expression)
		{
			expression=null;
			Expression? instance=null;

			var fieldInfo = type.GetField(name, bindingFlags);
			if (fieldInfo!= null)
			{
				var fieldInstance=Expression.Field(null,fieldInfo);

				// If it's an array then all is well
				if(fieldInfo.FieldType.IsArray)
				{
					expression=Expression.ArrayAccess(fieldInstance,indexes);
					return true;
				}

				// It's a type, so we call the default member
				string defaultMemberName=fieldInfo.FieldType.DefaultMemberName();
				if(defaultMemberName==null) return false;

				name=defaultMemberName;
				instance=fieldInstance;
			}
			else
			{
				var propertyInfo=type.GetProperty(name,bindingFlags);
				if (propertyInfo==null) return false;

				var propertyInstance=Expression.Property(null,propertyInfo);
				if (propertyInfo.PropertyType.IsArray)
				{
					expression=Expression.ArrayAccess(propertyInstance,indexes);
					return true;
				}

				// It's a type, so we call the default member
				string defaultMemberName=propertyInfo.PropertyType.DefaultMemberName();
				if(defaultMemberName==null) return false;

				name=defaultMemberName;
				instance=propertyInstance;
			}

			expression=Expression.Property(instance,name,indexes.ToArray());
			return true;
		} 
	}
}
