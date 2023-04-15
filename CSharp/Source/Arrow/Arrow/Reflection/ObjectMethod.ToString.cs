using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Reflection
{
    public static partial class ObjectMethod
    {
        /// <summary>
        /// Generates a function to render a type as a string
        /// </summary>
        /// <typeparam name="T">The type to generate the string for</typeparam>
        /// <returns>A function that generates a string</returns>
        public static Func<T, string> MakeToString<T>()
        {
            var properties = typeof(T).GetProperties(PublicInstance);

            var body = new ExpressionBlock();
            var @this = Expression.Parameter(typeof(T));

            // We'll build the results up into a StringBuilder
            var stringBuilder = Expression.Variable(typeof(StringBuilder));
            body.Add(Expression.Assign(stringBuilder, Expression.New(typeof(StringBuilder))));

            var appendString = typeof(StringBuilder).GetExactMethod("Append", PublicInstance, typeof(string))!;
            var appendObject = typeof(StringBuilder).GetExactMethod("Append", PublicInstance, typeof(object))!;

            bool appendComma = false;

            foreach(var property in properties)
            {
                // Make sure we put a comma in the right place
                if(appendComma)
                {
                    body.Add(Expression.Call(stringBuilder, appendString, Expression.Constant(", ")));
                }

                // This is the "Property=" part
                body.Add
                (
                    Expression.Call(stringBuilder, appendString, Expression.Constant(property.Name + "="))
                );

                Expression? append = null;

                if(property.PropertyType.IsValueType)
                {
                    // StringBuilder has a load of overloads for value types (int, double etc), so see if we can find one
                    var appendValue = typeof(StringBuilder).GetExactMethod("Append", PublicInstance, property.PropertyType);
                    if(appendValue != null)
                    {
                        append = Expression.Call(stringBuilder, appendValue, Expression.Property(@this, property));
                    }
                    else
                    {
                        // There's nothing specific, so call Append(object)
                        append = Expression.Call
                        (
                            stringBuilder,
                            appendObject,
                            Expression.Convert(Expression.Property(@this, property), typeof(object))
                        );
                    }
                }
                else
                {
                    // It's a reference type
                    if(property.PropertyType == typeof(string))
                    {
                        append = Expression.Call(stringBuilder, appendString, Expression.Property(@this, property));
                    }
                    else
                    {
                        append = Expression.Call(stringBuilder, appendObject, Expression.Property(@this, property));
                    }
                }

                body.Add(append);

                appendComma = true;
            }

            // That's all the properties, so now we call StringBuilder.ToString() to get the result
            var toString = typeof(StringBuilder).GetExactMethod("ToString", PublicInstance)!;
            body.Add(Expression.Call(stringBuilder, toString));

            var block = Expression.Block(AsSequence(stringBuilder), body);
            var function = Expression.Lambda<Func<T, string>>(block, @this).Compile();

            return function;
        }
    }
}
