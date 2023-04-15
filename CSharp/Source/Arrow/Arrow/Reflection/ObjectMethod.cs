using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Reflection
{
    /// <summary>
    /// Dynamically generates implementations of the overridable methods in System.Object
    /// </summary>
    public static partial class ObjectMethod
    {
        private static BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance;
        private static BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static;

        private static readonly Expression NullExpression = Expression.Constant(null, typeof(object));

        private static readonly Expression TrueExpression = Expression.Constant(true);
        private static readonly Expression FalseExpression = Expression.Constant(false);

        private static MethodInfo ReferenceEqualsMethod = typeof(object).GetMethod("ReferenceEquals", PublicStatic)!;

        private static IEnumerable<T> AsSequence<T>(params T[] expressions) where T : Expression
        {
            return expressions;
        }
    }
}
