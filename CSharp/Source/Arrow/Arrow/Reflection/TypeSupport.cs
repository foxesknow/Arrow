using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Reflection
{
    /// <summary>
    /// Useful type methods
    /// </summary>
    public static class TypeSupport
    {
        /// <summary>
        /// Finds the most specific type that 2 types have in common
        /// If a single type cannot be determines then null is returned
        /// </summary>
        /// <param name="lhs">The first type to check</param>
        /// <param name="rhs">The second type to check</param>
        /// <returns></returns>
        public static Type? MostSpecificType(Type lhs, Type rhs)
        {
            if(lhs == null) throw new ArgumentNullException("lhs");
            if(rhs == null) throw new ArgumentNullException("rhs");

            if(lhs == typeof(void) || rhs == typeof(void)) return null;
            if(lhs == rhs) return lhs;
            if(lhs == typeof(object) || rhs == typeof(object)) return typeof(object);

            var allLhs = lhs.GetBasesAndInterfaces();
            var allRhs = rhs.GetBasesAndInterfaces();

            // There'll always be at least one (ie object)
            var common = allLhs.Intersect(allRhs);

            Type? candidate = null;

            foreach(var type in common)
            {
                if(candidate == null)
                {
                    candidate = type;
                }
                else
                {
                    if(candidate.IsAssignableFrom(type))
                    {
                        // The new type is more specific
                        candidate = type;
                    }
                    else if(type.IsAssignableFrom(candidate))
                    {
                        // The type is less specific
                        continue;
                    }
                    else
                    {
                        // It's ambigious
                        candidate = null;
                        break;
                    }
                }
            }

            return candidate;
        }
    }
}
