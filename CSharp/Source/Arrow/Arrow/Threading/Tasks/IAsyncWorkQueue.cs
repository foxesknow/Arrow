using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Threading.Tasks
{
    public interface IAsyncWorkQueue
    {
        Task EnqueueAsync(Action action);
        (bool Enqueued, Task Task) TryEnqueueAsync(Action action);

        Task EnqueueAsync<T>(T state, Action<T> action);
        (bool Enqueued, Task Task) TryEnqueueAsync<T>(T state, Action<T> action);

        Task<TResult> EnqueueAsync<TResult>(Func<TResult> function);
        (bool Enqueued, Task<TResult> Task) TryEnqueueAsync<TResult>(Func<TResult> function);

        Task<TResult> EnqueueAsync<TState, TResult>(TState state, Func<TState, TResult> function);
        (bool Enqueued, Task<TResult> Task) TryEnqueueAsync<TState, TResult>(TState state, Func<TState, TResult> function);

        Task EnqueueAsync(Func<Task> function);
        (bool Enqueued, Task Task) TryEnqueueAsync(Func<Task> function);

        Task EnqueueAsync<TState>(TState state, Func<TState, Task> function);
        (bool Enqueued, Task Task) TryEnqueueAsync<TState>(TState state, Func<TState, Task> function);

        Task<TResult> EnqueueAsync<TResult>(Func<Task<TResult>> function);
        (bool Enqueued, Task<TResult> Task) TryEnqueueAsync<TResult>(Func<Task<TResult>> function);

        Task<TResult> EnqueueAsync<TState, TResult>(TState state, Func<TState, Task<TResult>> function);
        (bool Enqueued, Task<TResult> Task) TryEnqueueAsync<TState, TResult>(TState state, Func<TState, Task<TResult>> function);
    }
}
