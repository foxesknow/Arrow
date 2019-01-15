using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Threading.Tasks
{
    public interface IAsyncWorkQueue
    {
        /// <summary>
        /// Enqueues an item for execution
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        Task EnqueueAsync(Action action);

        /// <summary>
        /// Attempts to enqueue an item for execution
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        (bool Enqueued, Task Task) TryEnqueueAsync(Action action);

        /// <summary>
        /// Enqueues an item for execution
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="state"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        Task EnqueueAsync<T>(T state, Action<T> action);

        /// <summary>
        /// Attempts to enqueue an item for execution
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="state"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        (bool Enqueued, Task Task) TryEnqueueAsync<T>(T state, Action<T> action);

        /// <summary>
        /// Enqueues an item for execution
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="function"></param>
        /// <returns></returns>
        Task<TResult> EnqueueAsync<TResult>(Func<TResult> function);

        /// <summary>
        /// Attempts to enqueue an item for execution
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="function"></param>
        /// <returns></returns>
        (bool Enqueued, Task<TResult> Task) TryEnqueueAsync<TResult>(Func<TResult> function);

        /// <summary>
        /// Enqueues an item for execution
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="state"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        Task<TResult> EnqueueAsync<TState, TResult>(TState state, Func<TState, TResult> function);

        /// <summary>
        /// Attempts to enqueue an item for execution
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="state"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        (bool Enqueued, Task<TResult> Task) TryEnqueueAsync<TState, TResult>(TState state, Func<TState, TResult> function);

        /// <summary>
        /// Enqueues an item for execution
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        Task EnqueueAsync(Func<Task> function);

        /// <summary>
        /// Attempts to enqueue an item for execution
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        (bool Enqueued, Task Task) TryEnqueueAsync(Func<Task> function);

        /// <summary>
        /// Enqueues an item for execution
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="state"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        Task EnqueueAsync<TState>(TState state, Func<TState, Task> function);

        /// <summary>
        /// Attempts to enqueue an item for execution
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="state"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        (bool Enqueued, Task Task) TryEnqueueAsync<TState>(TState state, Func<TState, Task> function);

        /// <summary>
        /// Enqueues an item for execution
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="function"></param>
        /// <returns></returns>
        Task<TResult> EnqueueAsync<TResult>(Func<Task<TResult>> function);

        /// <summary>
        /// Attempts to enqueue an item for execution
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="function"></param>
        /// <returns></returns>
        (bool Enqueued, Task<TResult> Task) TryEnqueueAsync<TResult>(Func<Task<TResult>> function);

        /// <summary>
        /// Enqueues an item for execution
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="state"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        Task<TResult> EnqueueAsync<TState, TResult>(TState state, Func<TState, Task<TResult>> function);

        /// <summary>
        /// /// Attempts to enqueue an item for execution
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="state"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        (bool Enqueued, Task<TResult> Task) TryEnqueueAsync<TState, TResult>(TState state, Func<TState, Task<TResult>> function);
    }
}
