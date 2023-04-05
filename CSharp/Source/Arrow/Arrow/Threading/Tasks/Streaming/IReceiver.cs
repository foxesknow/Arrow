using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Threading.Tasks.Streaming
{
    /// <summary>
    /// A receiver is a type that is given data and allows someone to receive that data
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public interface IReceiver<TData>
    {
        /// <summary>
        /// How long to wait is a specific timeout is not specified
        /// </summary>
        public TimeSpan DefaultTimeout { get; }

        /// <summary>
        /// Waits for the condition to be satisfied and then execites an action.
        /// The matching data is removed from the receiver
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="ifCondition"></param>
        /// <param name="then"></param>
        /// <returns></returns>
        Task<TData> WaitFor(TimeSpan timeout, Func<TData, bool> ifCondition, Action<TData> then);
    }

    public static class ReceiverExtensions
    {
        private static class EmptyThen<T>
        {
            public static readonly Action<T> Instance = static d => { };
        }

        /// <summary>
        /// Wait for a condtion to be satisfied
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="receiver"></param>
        /// <param name="ifCondition"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Task<TData> WaitFor<TData>(this IReceiver<TData> receiver, Func<TData, bool> ifCondition)
        {
            if (receiver is null) throw new ArgumentNullException(nameof(receiver));

            return receiver.WaitFor(receiver.DefaultTimeout, ifCondition, EmptyThen<TData>.Instance);
        }

        /// <summary>
        /// Wait for a condition to be satisfied
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="receiver"></param>
        /// <param name="timeout"></param>
        /// <param name="ifCondition"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Task<TData> WaitFor<TData>(this IReceiver<TData> receiver, TimeSpan timeout, Func<TData, bool> ifCondition)
        {
            if (receiver is null) throw new ArgumentNullException(nameof(receiver));

            return receiver.WaitFor(timeout, ifCondition, EmptyThen<TData>.Instance);
        }

        /// <summary>
        /// Waits for a condition to be satisfied and then executes an action
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="receiver"></param>
        /// <param name="timeout"></param>
        /// <param name="ifCondition"></param>
        /// <param name="then"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Task<TData> WaitFor<TData>(this IReceiver<TData> receiver, TimeSpan timeout, Func<TData, bool> ifCondition, Action<TData> then)
        {
            if (receiver is null) throw new ArgumentNullException(nameof(receiver));

            return receiver.WaitFor(timeout, ifCondition, then);
        }
    }
}
