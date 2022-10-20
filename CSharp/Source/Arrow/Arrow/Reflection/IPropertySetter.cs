using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Reflection
{
    public interface IPropertySetter<T>
    {
        /// <summary>
        /// The object whose properties will be set
        /// </summary>
        public T Object{get;}

        /// <summary>
        /// Sets a property by textual name.
        /// If the type of the value is not the same as the type of the property then a conversion will be attempted
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
        public IPropertySetter<T> Set(string propertyName, object? propertyValue);
        
        /// <summary>
        /// Sets a property using a lambda to determine the type
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="property"></param>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
        public IPropertySetter<T> Set<R>(Expression<Func<T, R>> property, R propertyValue);
    }
}
