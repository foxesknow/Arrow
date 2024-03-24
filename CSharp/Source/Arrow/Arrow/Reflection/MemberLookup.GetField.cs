using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Reflection
{
    public static partial class MemberLookup
    {
        /// <summary>
        /// Extracts the FieldInfo from a lamda representing a field access.
        /// This method is for getting static fields
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="delegate"></param>
        /// <returns></returns>
        public static FieldInfo GetField<TOut>(Expression<Func<TOut>> @delegate)
        {
            return DoGetField(@delegate);
        }

        /// <summary>
        /// Extracts the FieldInfo from a lamda representing a field access.
        /// This method is for getting instance fields
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="delegate"></param>
        /// <returns></returns>
        public static FieldInfo GetField<TIn, TOut>(Expression<Func<TIn, TOut>> @delegate)
        {
            return DoGetField(@delegate);
        }

        private static FieldInfo DoGetField<T>(Expression<T> @delegate) where T : Delegate
        {
            ArgumentNullException.ThrowIfNull(@delegate);

            if(@delegate is LambdaExpression lambda && lambda.Body is MemberExpression member && member.Member is FieldInfo field)
            {
                return field;
            }

            throw new ArgumentException("not a field ", nameof(@delegate));
        }
    }
}
