using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Arrow.Reflection;

namespace Arrow.Net
{
    public class QueryString
    {
        /// <summary>
        /// Applies the query parameters to a target object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static T Apply<T>(Uri uri, T target) where T : class
        {
            if(uri is null) throw new ArgumentNullException(nameof(uri));
            if(target is null) throw new ArgumentNullException(nameof(target));

            var queryParts = uri.ParseQuery();
            var type = typeof(T);

            foreach(var pair in queryParts)
            {
                var key = pair.Key;
                if(key is null) continue;

                var property = type.GetProperty(key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                if(property is null) continue;
                if(property.CanWrite == false) continue;

                var value = pair.Value;
                var resolvedValue = TypeResolver.CoerceToType(property.PropertyType, value);
                property.SetValue(target, resolvedValue);
            }

            return target;
        }
    }
}
