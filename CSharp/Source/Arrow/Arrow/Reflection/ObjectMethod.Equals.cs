using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Reflection
{
    public static partial class ObjectMethod
    {
        /// <summary>
        /// Generates a function to test for equality using the public properties of a type
        /// </summary>
        /// <typeparam name="T">The type to generate the function for</typeparam>
        /// <returns>A function that checks for equality</returns>
        public static Func<T, T, bool> MakeEquals<T>()
        {
            var @this = Expression.Parameter(typeof(T));
            var other = Expression.Parameter(typeof(T));

            var andChain = TrueExpression;

            var properties = typeof(T).GetProperties(PublicInstance);

            if(properties.Length == 0)
            {
                // No properties, so we'll only do the ReferencEquals checks
                andChain = FalseExpression;
            }
            else
            {
                // We need to build up a chain of comparison
                // EQ: (this.X==other.X) && (this.Y==other.Y) etc
                foreach(var property in properties)
                {
                    var compare = Compare(@this, other, property);
                    andChain = Expression.AndAlso(andChain, compare);
                }
            }

            // Finally, check for reference equality: if(object.ReferenceEquals(this,other)) return true
            var identityCheck = Expression.Condition
            (
                Expression.Call(ReferenceEqualsMethod, @this, other),
                TrueExpression,
                andChain
            );

            // Check for null: if(object.ReferenceEquals(other,null)) return false
            var checkForNull = Expression.Condition
            (
                Expression.Call(ReferenceEqualsMethod, other, NullExpression),
                FalseExpression,
                identityCheck
            );

            var function = Expression.Lambda<Func<T, T, bool>>(checkForNull, AsSequence(@this, other)).Compile();
            return function;
        }

        private static Expression Compare(Expression lhs, Expression rhs, PropertyInfo property)
        {
            var type = property.PropertyType;

            // Read the property values
            var lValue = Expression.Property(lhs, property);
            var rValue = Expression.Property(rhs, property);

            // We'll use the EqualityComparer to simplify things
            var comparer = typeof(EqualityComparer<>).MakeGenericType(type);
            var equalsMethod = comparer.GetMethod("Equals", new Type[] { type, type })!;

            var defaultProperty = comparer.GetProperty("Default", PublicStatic)!;
            var @default = Expression.Property(null, defaultProperty);

            return Expression.Call(@default, equalsMethod, lValue, rValue);
        }
    }
}
