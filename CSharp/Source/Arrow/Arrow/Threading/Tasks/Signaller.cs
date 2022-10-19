using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace Arrow.Threading.Tasks
{
    /// <summary>
    /// Manages a list of conditions that will signal a task when the conditon is met.
    /// Once a condition is met the condition will be removed from the signaller.
    /// This class is thread safe
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed partial class Signaller<T>
    {
        private long m_ID;
        private readonly ConcurrentDictionary<long, State> m_State = new();

        /// <summary>
        /// Accepts data and signals any conditions.
        /// Any conditions that match the data will be removed.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Information on what has been signalled</returns>
        public SignalResult Signal(T data)
        {
            if(m_State.Count == 0) return default;

            var successful = 0;
            var failed = 0;
            var threw = 0;

            foreach(var pair in m_State)
            {
                var id = pair.Key;
                var state = pair.Value;

                var remove = false;
                try
                {
                    if(state.Condition(data))
                    {
                        if(state.Tcs.TrySetResult(data))
                        {
                            successful++;
                        }
                        else
                        {
                            failed++;
                        }

                        remove = true;
                    }
                }
                catch(Exception e)
                {
                    state.Tcs.TrySetException(e);
                    threw++;
                    remove = true;
                }

                if(remove) m_State.TryRemove(id, out var _);
            }

            return new(successful, failed, threw);
        }

        /// <summary>
        /// Attempts to remove a task.
        /// The state of the task will not be altered
        /// </summary>
        /// <param name="task">The task to remove</param>
        /// <returns>true if the task was removed, otherwise false</returns>
        public bool Remove(Task<T> task)
        {
            if(task is null) throw new ArgumentNullException(nameof(task));

            foreach(var pair in m_State)
            {
                var state = pair.Value;
                if(object.ReferenceEquals(task, state.Task))
                {
                    return m_State.TryRemove(pair.Key, out var _);
                }
            }

            return false;
        }

        /// <summary>
        /// Attempts to remove a task via a WhenToken.
        /// This method is usually called by implementations building on the signaller primitives.
        /// </summary>
        /// <param name="whenToken"></param>
        /// <returns>true if the task was removed, otherwise false</returns>
        public bool Remove(WhenToken whenToken)
        {
            if(whenToken.ID != 0 && whenToken.Task is not null)
            {
                return m_State.TryRemove(whenToken.ID, out var _);
            }

            return false;
        }

        /// <summary>
        /// Attempts to cancel the specified task.
        /// If the task is present it will be cancelled. This will cause an OperationCanceledException be be
        /// observed by anyone awaiting the task
        /// </summary>
        /// <param name="task"></param>
        /// <returns>true if the task was found and cancelled, otherwise false</returns>
        public bool Cancel(Task<T> task)
        {
            if(task is null) throw new ArgumentNullException(nameof(task));

            foreach(var pair in m_State)
            {
                var state = pair.Value;
                if(object.ReferenceEquals(task, state.Task))
                {
                    m_State.TryRemove(pair.Key, out var _);
                    return state.Tcs.TrySetCanceled();
                }
            }

            return false;
        }

        /// <summary>
        /// Attemps to cancel a task via a WhenToken.
        /// This method is usually called by implementations building on the signaller primitives.
        /// If the task is present it will be cancelled. This will cause an OperationCanceledException be be
        /// observed by anyone awaiting the task
        /// </summary>
        /// <param name="whenToken"></param>
        /// <returns>true if the task was found and cancelled, otherwise false</returns>
        public bool Cancel(WhenToken whenToken)
        {
            if(whenToken.ID != 0 && whenToken.Task is not null)
            {
                if(m_State.TryRemove(whenToken.ID, out var state))
                { 
                    return state.Tcs.TrySetCanceled();
                }
            }

            return false;
        }

        /// <summary>
        /// Cancels all conditions
        /// This will cause an OperationCanceledException be be observed by anyone awaiting any
        /// if the tasks that are cancelled
        /// </summary>
        /// <returns>The number of cancelled tasks</returns>
        public int CancelAll()
        {
            var cancelled = 0;

            foreach(var pair in m_State)
            {
                var state = pair.Value;
                if(state.Tcs.TrySetCanceled())
                {
                    cancelled++;
                }

                m_State.TryRemove(pair.Key, out var _);
            }

            return cancelled;
        }

        /// <summary>
        /// Adds a condition to the signaller
        /// </summary>
        /// <param name="condition"></param>
        /// <returns>A task that will be signalled when the condition evaluates to true</returns>
        public Task<T> When(Func<T, bool> condition)
        {
            var token = ImplementationWhen(condition);
            return token.Task;
        }

        /// <summary>
        /// Adds a condition to the signaller.
        /// This method is called by implementations that want to optimize the use of the signaller
        /// </summary>
        /// <param name="condition"></param>
        /// <returns>A token containing a task and a unique ID for the registration</returns>
        /// <exception cref="ArgumentNullException">condition is null</exception>
        public WhenToken ImplementationWhen(Func<T, bool> condition)
        {
            if(condition is null) throw new ArgumentNullException(nameof(condition));

            var state = RegisterCondition(condition);
            return new WhenToken(state.Task, state.ID);
        }

        /// <summary>
        /// Registers a condition to wait for and then executes an action
        /// </summary>
        /// <param name="waitCondition">The condition to wait for</param>
        /// <param name="action">An action that typically does something that may trigger the condition</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Task<T> ExecuteAndWait(Func<T, bool> waitCondition, Action action)
        {
            if(waitCondition is null) throw new ArgumentNullException(nameof(waitCondition));
            if(action is null) throw new ArgumentNullException(nameof(action));

            return ExecuteAndWait(waitCondition, Execute);

            Task Execute()
            {
                action();
                return Task.CompletedTask;
            }
        }

        /// <summary>
        /// Registers a condition to wait for and then executes a function
        /// </summary>
        /// <param name="waitCondition">The condition to wait for</param>
        /// <param name="function">A function that typically does something that may trigger the condition</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Task<T> ExecuteAndWait(Func<T, bool> waitCondition, Func<Task> function)
        {
            if(waitCondition is null) throw new ArgumentNullException(nameof(waitCondition));
            if(function is null) throw new ArgumentNullException(nameof(function));

            return Execute();

            async Task<T> Execute()
            {
                var state = RegisterCondition(waitCondition);

                try
                {
                    await function().ContinueOnAnyContext();
                    var result = await state.Task.ContinueOnAnyContext();

                    return result;
                }
                catch(Exception)
                {
                    m_State.TryRemove(state.ID, out var _);
                    throw;
                }
            }
        }

        private State RegisterCondition(Func<T, bool> condition)
        {
            var id = MakeID();
            var state = new State(id, condition);
            m_State.TryAdd(id, state);

            return state;
        }

        private long MakeID()
        {
            return Interlocked.Increment(ref m_ID);
        }
    }
}
