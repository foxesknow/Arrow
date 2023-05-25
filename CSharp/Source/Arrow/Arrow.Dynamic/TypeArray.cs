using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Dynamic
{
    public static class TypeArray
    {
        /// <summary>
        /// Creates an array of types
        /// </summary>
        /// <typeparam name="T1">The first type</typeparam>
        /// <returns>A type array</returns>
        public static Type[] Make<T1>()
        {
            return new Type[]{typeof(T1)};
        }

        /// <summary>
        /// Creates an array of types
        /// </summary>
        /// <typeparam name="T1">The first type</typeparam>
        /// <typeparam name="T2">The second type</typeparam>
        /// <returns>A type array</returns>
        public static Type[] Make<T1, T2>()
        {
            return new Type[]{typeof(T1), typeof(T2)};
        }

        /// <summary>
        /// Creates an array of types
        /// </summary>
        /// <typeparam name="T1">The first type</typeparam>
        /// <typeparam name="T2">The second type</typeparam>
        /// <typeparam name="T3">The third type</typeparam>
        /// <returns>A type array</returns>
        public static Type[] Make<T1, T2, T3>()
        {
            return new Type[]{typeof(T1), typeof(T2), typeof(T3)};
        }

        /// <summary>
        /// Creates an array of types
        /// </summary>
        /// <typeparam name="T1">The first type</typeparam>
        /// <typeparam name="T2">The second type</typeparam>
        /// <typeparam name="T3">The third type</typeparam>
        /// <typeparam name="T4">The fourth type</typeparam>
        /// <returns>A type array</returns>
        public static Type[] Make<T1, T2, T3, T4>()
        {
            return new Type[]{typeof(T1), typeof(T2), typeof(T3), typeof(T4)};
        }

        /// <summary>
        /// Creates an array of types
        /// </summary>
        /// <typeparam name="T1">The first type</typeparam>
        /// <typeparam name="T2">The second type</typeparam>
        /// <typeparam name="T3">The third type</typeparam>
        /// <typeparam name="T4">The fourth type</typeparam>
        /// <typeparam name="T5">The fifth type</typeparam>
        /// <returns>A type array</returns>
        public static Type[] Make<T1, T2, T3, T4, T5>()
        {
            return new Type[]{typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5)};
        }

        /// <summary>
        /// Creates an array of types
        /// </summary>
        /// <typeparam name="T1">The first type</typeparam>
        /// <typeparam name="T2">The second type</typeparam>
        /// <typeparam name="T3">The third type</typeparam>
        /// <typeparam name="T4">The fourth type</typeparam>
        /// <typeparam name="T5">The fifth type</typeparam>
        /// <typeparam name="T6">The sixth type</typeparam>
        /// <returns>A type array</returns>
        public static Type[] Make<T1, T2, T3, T4, T5, T6>()
        {
            return new Type[]{typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6)};
        }

        /// <summary>
        /// Creates an array of types
        /// </summary>
        /// <typeparam name="T1">The first type</typeparam>
        /// <typeparam name="T2">The second type</typeparam>
        /// <typeparam name="T3">The third type</typeparam>
        /// <typeparam name="T4">The fourth type</typeparam>
        /// <typeparam name="T5">The fifth type</typeparam>
        /// <typeparam name="T6">The sixth type</typeparam>
        /// <typeparam name="T7">The seventh type</typeparam>
        /// <returns>A type array</returns>
        public static Type[] Make<T1, T2, T3, T4, T5, T6, T7>()
        {
            return new Type[]{typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7)};
        }

        /// <summary>
        /// Creates an array of types
        /// </summary>
        /// <typeparam name="T1">The first type</typeparam>
        /// <typeparam name="T2">The second type</typeparam>
        /// <typeparam name="T3">The third type</typeparam>
        /// <typeparam name="T4">The fourth type</typeparam>
        /// <typeparam name="T5">The fifth type</typeparam>
        /// <typeparam name="T6">The sixth type</typeparam>
        /// <typeparam name="T7">The seventh type</typeparam>
        /// <typeparam name="T8">The eight type</typeparam>
        /// <returns>A type array</returns>
        public static Type[] Make<T1, T2, T3, T4, T5, T6, T7, T8>()
        {
            return new Type[]{typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8)};
        }
    }
}
