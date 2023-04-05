using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Threading.Tasks.SignallerSwitch
{
    public static class SignallerSwitchExtensions
    {
        /// <summary>
        /// Executes an action and then allows you to switch on something thay may be signalled.
        /// If you don't have an initial function to execute then pass a lambda that returns Task.CompletedTask.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="signaller"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Implementation.SignallerSwitchImplementation.InitialCase<T, Unit> Switch<T>(this Signaller<T> signaller, Action<Unit> action)
        {
            if(action is null) throw new ArgumentNullException(nameof(action));

            Func<Unit, Task> function = state =>
            {
                action(state);
                return Task.CompletedTask;
            };

            return Switch<T, Unit>(signaller, Unit.Default, function);
        }

        /// <summary>
        /// Executes an action and then allows you to switch on something thay may be signalled.
        /// If you don't have an initial function to execute then pass a lambda that returns Task.CompletedTask.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="signaller"></param>
        /// <param name="state"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Implementation.SignallerSwitchImplementation.InitialCase<T, TState> Switch<T, TState>(this Signaller<T> signaller, TState state, Action<TState> action)
        {
            if(action is null) throw new ArgumentNullException(nameof(action));

            Func<TState, Task> function = state =>
            {
                action(state);
                return Task.CompletedTask;
            };

            return Switch(signaller, state, function);
        }

        /// <summary>
        /// Executes a function and then allows you to switch on something thay may be signalled.
        /// If you don't have an initial function to execute then pass a lambda that returns Task.CompletedTask.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="signaller"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        public static Implementation.SignallerSwitchImplementation.InitialCase<T, Unit> Switch<T>(this Signaller<T> signaller, Func<Unit, Task> function)
        {
            return Switch<T, Unit>(signaller, Unit.Default, function);
        }

        /// <summary>
        /// Executes a function and then allows you to switch on something thay may be signalled.
        /// If you don't have an initial function to execute then pass a lambda that returns Task.CompletedTask.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TState"></typeparam>
        /// <param name="signaller"></param>
        /// <param name="state">Optional state to pass to the case statements and function</param>
        /// <param name="function">The function to execute before evaluating the case statements</param>
        /// <returns>An object that allows you to add case statements</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Implementation.SignallerSwitchImplementation.InitialCase<T, TState> Switch<T, TState>(this Signaller<T> signaller, TState state, Func<TState, Task> function)
        {
            if(signaller is null) throw new ArgumentNullException(nameof(signaller));
            if(function is null) throw new ArgumentNullException(nameof(function));

            return new(signaller, state, function);
        }
    }
}
