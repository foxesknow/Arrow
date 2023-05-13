using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Execution;

namespace Arrow.Functional
{
    public static class OptionExtensions
    {
        /// <summary>
        /// Extracts an option from another option
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Option<T> Flatten<T>(in this Option<Option<T>> self)
        {
            return self.ValueOr(Option.None);
        }

        /// <summary>
        /// Converts an option to a nullable value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="_">Ensure the method is called only for value types. Just accept the default value</param>
        /// <returns></returns>
        public static T? ToNullable<T>(in this Option<T> self, RequireStruct<T>? _ = null) where T : struct
        {
            if (self.IsSome)
            {
                return self.Value();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Converts an option to a nullable reference
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="_">Ensure the method is called only for reference types. Just accept the default value</param>
        /// <returns></returns>
        public static T? ToNullable<T>(in this Option<T> self, RequireClass<T>? _ = null) where T : class
        {
            if (self.IsSome)
            {
                return self.Value();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns self if it is some, otherwise returns other
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static Option<T> OrElse<T>(in this Option<T> self, in Option<T> other)
        {
            return self.IsSome ? self : other;
        }

        /// <summary>
        /// Returns self if it is some, otherwise calls a function to get an alternative value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Option<T> OrElse<T>(in this Option<T> self, Func<Option<T>> function)
        {
            if (function is null) throw new ArgumentNullException(nameof(function));

            return self.IsSome ? self : function();
        }

        /// <summary>
        /// Returns self if it is some, otherwise calls a function to get an alternative value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TState"></typeparam>
        /// <param name="self"></param>
        /// <param name="state">Additional state to pass to the function</param>
        /// <param name="function"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Option<T> OrElse<T, TState>(in this Option<T> self, TState state, Func<TState, Option<T>> function)
        {
            if (function is null) throw new ArgumentNullException(nameof(function));

            return self.IsSome ? self : function(state);
        }

        /// <summary>
        /// This SelectMany extension method enables Linq support
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="self"></param>
        /// <param name="optionSelector"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public static Option<TResult> SelectMany<T, U, TResult>(in this Option<T> self, Func<T, Option<U>> optionSelector, Func<T, U, TResult> resultSelector)
        {
            return self.Bind
            (
                (optionSelector, resultSelector),
                static (t, state) => state.optionSelector(t).Select
                (
                    (state.resultSelector, t),
                    static (u, state) => state.resultSelector(state.t, u)
                )
            );
        }
    }
}
