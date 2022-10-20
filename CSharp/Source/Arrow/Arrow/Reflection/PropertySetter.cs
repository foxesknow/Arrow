using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace Arrow.Reflection
{
    /// <summary>
    /// Allows you to set properties on an instance using reflection.
    /// The property must have a public getter and a setter (even if the setter is not public)
    /// </summary>
    public static class PropertySetter
    {
        public static IPropertySetter<T> Make<T>(T instance) where T : class
        {
            return new PropertySetterImpl<T>(instance);
        }

        private static void Set<T>(T instance, string name, object? value) where T : class
        {
            if(name == null) throw new ArgumentNullException(nameof(name));
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("invalid property name", nameof(name));

            var property = instance.GetType().GetProperty(name);
            if(property is null) throw new InvalidOperationException($"no such property: {name}");

            Set(instance, property, value, true);
        }

        private static void Set<T, R>(T instance, Expression<Func<T, R>> propertyFunction, R propertyValue) where T : class
        {
            if(propertyFunction is null) throw new ArgumentNullException(nameof(propertyFunction));

            var member = propertyFunction.Body as MemberExpression;
            if(member is null) throw new InvalidOperationException($"expression {propertyFunction.ToString()} is not a property");

            var property = member.Member as PropertyInfo;
            if(property is null) throw new InvalidOperationException($"expression {propertyFunction.ToString()} is not a property");

            Set(instance, property, propertyValue, false);
        }

        private static void Set(object instance, PropertyInfo property, object? value, bool coerceValue)
        {
            if(property.CanWrite == false) throw new InvalidOperationException($"property is read only: {property.Name}");

            if(coerceValue)
            {
                var coercedValue = TypeResolver.CoerceToType(property.PropertyType, value);
                property.SetValue(instance, coercedValue);
            }
            else
            {
                property.SetValue(instance, value);
            }
        }

        private class PropertySetterImpl<T> : IPropertySetter<T> where T : class
        {
            public PropertySetterImpl(T instance)
            {
                this.Object = instance;
            }

            public T Object{get;}

            public IPropertySetter<T> Set(string propertyName, object? propertyValue)
            {
                PropertySetter.Set(this.Object, propertyName, propertyValue);
                return this;
            }

            public IPropertySetter<T> Set<R>(System.Linq.Expressions.Expression<Func<T, R>> property, R propertyValue)
            {
                PropertySetter.Set(this.Object, property, propertyValue);
                return this;
            }
        }
    }
}
