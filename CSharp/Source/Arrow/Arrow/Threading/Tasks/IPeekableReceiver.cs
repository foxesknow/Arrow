using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Threading.Tasks
{
    /// <summary>
    /// A peekable receiver is a type that is given data and allows someone to receive that data,
    /// or to peek to see if the data is available
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public interface IPeekableReceiver<TData> : IReceiver<TData>
    {
        Task<TData> PeekFor(TimeSpan timeout, Func<TData, bool> ifCondition, Action<TData> then);

        public bool TryPeek(out TData data, Func<TData, bool> condition);
    }

    public static class PeekableReceiverExtensions
    {
        private static class EmptyThen<T>
        {
            public static readonly Action<T> Instance = static d => {};
        }

        public static Task<TData> PeekFor<TData>(this IPeekableReceiver<TData> receiver, Func<TData, bool> ifCondition)
        {
            if(receiver is null) throw new ArgumentNullException(nameof(receiver));

            return receiver.PeekFor(receiver.DefaultTimeout, ifCondition, EmptyThen<TData>.Instance);
        }

        public static Task<TData> PeekFor<TData>(this IPeekableReceiver<TData> receiver, TimeSpan timeout, Func<TData, bool> ifCondition)
        {
            if(receiver is null) throw new ArgumentNullException(nameof(receiver));

            return receiver.PeekFor(timeout, ifCondition, EmptyThen<TData>.Instance);
        }

        public static Task<TData> PeekFor<TData>(this IPeekableReceiver<TData> receiver, TimeSpan timeout, Func<TData, bool> ifCondition, Action<TData> then)
        {
            if(receiver is null) throw new ArgumentNullException(nameof(receiver));

            return receiver.PeekFor(timeout, ifCondition,then);
        }
    }
}
