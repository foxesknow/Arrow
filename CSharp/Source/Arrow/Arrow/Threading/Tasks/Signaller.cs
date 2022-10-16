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
    public sealed partial class Signaller<T>
    {
        private long m_ID;
        private readonly ConcurrentDictionary<long, State> m_State = new();

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

        public bool Remove(Task<T> task)
        {
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

        public bool Remove(WhenToken whenToken)
        {
            if(whenToken.ID != 0 && whenToken.Task is not null)
            {
                return m_State.TryRemove(whenToken.ID, out var _);
            }

            return false;
        }

        public bool Cancel(Task<T> task)
        {
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

        public Task<T> When(Func<T, bool> condition)
        {
            var token = ImplementationWhen(condition);
            return token.Task;
        }

        public WhenToken ImplementationWhen(Func<T, bool> condition)
        {
            if(condition is null) throw new ArgumentNullException(nameof(condition));

            var state = RegisterCondition(condition);
            return new WhenToken(state.Task, state.ID);
        }

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
