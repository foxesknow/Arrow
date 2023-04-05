using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Arrow.Execution;
using Arrow.Threading.Tasks.Streaming;

namespace Arrow.Threading.Tasks
{
    /// <summary>
    /// A class that holds data and will notify anyone waiting for data matching data to be added
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public sealed partial class DataBuffer<TData> : IPeekableReceiver<TData>, IPublisher<TData>
    {
        private readonly object m_SyncRoot = new object();

        private readonly List<TData> m_Buffer = new();
        private readonly List<ConditionState> m_WhenConditions = new();
        private readonly List<ConditionState> m_PeekConditions = new();

        /// <summary>
        /// Initializes the instance so that waits wait forever
        /// </summary>
        public DataBuffer() : this(DataTimeout.WaitForever)
        {
        }

        /// <summary>
        /// Initializes the buffer
        /// </summary>
        /// <param name="waitTimeout"></param>
        public DataBuffer(TimeSpan waitTimeout)
        {
            this.DefaultTimeout = waitTimeout;
        }

        /// <summary>
        /// How long to wait by default
        /// </summary>
        public TimeSpan DefaultTimeout{get;}

        /// <summary>
        /// True if there is data in the buffer, otherwise false
        /// </summary>
        public bool HasData
        {
            get
            {
                lock(m_SyncRoot)
                {
                    return m_Buffer.Count != 0;
                }
            }
        }

        /// <summary>
        /// Removes all data from teh buffer
        /// </summary>
        public void Clear()
        {
            lock(m_SyncRoot)
            {
                m_Buffer.Clear();
            }
        }

        /// <summary>
        /// Removes all data and cancels any waiting task
        /// Anyone waiting on the cancelled task will observe a OperationCanceledException
        /// </summary>
        public void Reset()
        {
            lock(m_SyncRoot)
            {
                Clear();
                CancelAll();
            }
        }

        /// <summary>
        /// Cancels all peek and wait conditions.
        /// Anyone waiting on the cancelled task will observe a OperationCanceledException
        /// </summary>
        public void CancelAll()
        {
            lock(m_SyncRoot)
            {
                Cancel(m_PeekConditions);
                Cancel(m_WhenConditions);
            }

            static void Cancel(IList<ConditionState> conditions)
            {
                if(conditions.Count == 0) return;

                for(int i = 0; i < conditions.Count; i++)
                {
                    conditions[i].Tcs.SetCanceled();
                }

                conditions.Clear();
            }
        }

        /// <summary>
        /// Cancels a specific wait or peek task.
        /// Anyone waiting on the cancelled task will observe a OperationCanceledException
        /// </summary>
        /// <param name="task"></param>
        /// <returns>true if cancelled, otherwis false</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool Cancel(Task<TData> task)
        {
            if(task is null) throw new ArgumentNullException(nameof(task));

            lock(m_SyncRoot)
            {
                return Cancel(task, m_PeekConditions) || Cancel(task, m_WhenConditions);
            }

            static bool Cancel(Task<TData> task, IList<ConditionState> conditions)
            {
                for(int i = 0; i < conditions.Count; i++)
                {
                    var tcs = conditions[i].Tcs;
                    if(tcs.Task == task)
                    {
                        conditions.RemoveAt(i);
                        tcs.SetCanceled();

                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Resets the buffer and returns a disposable instance that will reset the buffer when disposed.
        /// Anyone waiting on the cancelled task will observe a OperationCanceledException
        /// This is useful for testing discrete areas of code, such as in a test
        /// </summary>
        /// <returns></returns>
        public IDisposable StartActivity()
        {
            Reset();
            return new Disposer(() => Reset());
        }

        /// <summary>
        /// Publishes data
        /// Any conditions waiting for the data will be notified
        /// </summary>
        /// <param name="data"></param>
        public void Publish(TData data)
        {
            lock(m_SyncRoot)
            {
                if(ExecuteConditions(data) == ExecutionOutcome.NotConsumed)
                {
                    m_Buffer.Add(data);
                }
            }
        }

        /// <summary>
        /// Waits for data to arrive that satisfies a condition then then executes an action
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="ifCondition"></param>
        /// <param name="then"></param>
        /// <returns></returns>
        public Task<TData> WaitFor(TimeSpan timeout, Func<TData, bool> ifCondition, Action<TData> then)
        {
            return MakeTask(m_WhenConditions, timeout, ifCondition, then);
        }

        /// <summary>
        /// Waits for data to arrive that satisfies a condition then then executes an action.
        /// The data is not removed from the buffer
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="ifCondition"></param>
        /// <param name="then"></param>
        /// <returns></returns>
        public Task<TData> PeekFor(TimeSpan timeout, Func<TData, bool> ifCondition, Action<TData> then)
        {
            return MakeTask(m_PeekConditions, timeout, ifCondition, then);
        }
        
        /// <summary>
        /// Tries to see if there is any data in the buffer that satisfies the condition
        /// </summary>
        /// <param name="data"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool TryPeek([MaybeNullWhen(false)]out TData data, Func<TData, bool> condition)
        {
            if(condition is null) throw new ArgumentNullException(nameof(condition));

            lock(m_SyncRoot)
            {
                // Process in reverese to be consistent with Push()
                for(int i = m_Buffer.Count -1; i >= 0; i--)
                {
                    var item = m_Buffer[i];
                    if(condition(item))
                    {
                        data = item;
                        return true;
                    }
                }
            }

            data = default!;
            return false;
        }

        private Task<TData> MakeTask(IList<ConditionState> conditions, TimeSpan timeout, Func<TData, bool> ifCondition, Action<TData> then)
        {
            if(ifCondition is null) throw new ArgumentNullException(nameof(ifCondition));
            if(then is null) throw new ArgumentNullException(nameof(then));
            if(DataTimeout.IsValid(timeout) == false) throw new ArgumentException("invalid timeout", nameof(ifCondition));

            var task = Setup(conditions, ifCondition, then);

            // If Setup caused the condition to be met, or we're waiting forever then we're done
            if(task.IsCompleted || timeout == DataTimeout.WaitForever) return task;

            return Execute(task, timeout);

            static async Task<TData> Execute(Task<TData> task, TimeSpan timeout)
            {
                using(var cts = new CancellationTokenSource())
                {
                    var delayTask = Task.Delay(timeout, cts.Token);
                    await Task.WhenAny(task, delayTask).ContinueOnAnyContext();

                    if(task.IsCompleted)
                    {
                        // We need to cancel the delay
                        await TaskEx.CancelAndWait(cts, delayTask).ContinueOnAnyContext();
                        return await task;
                    }

                    throw new TimeoutException("condition timed out");
                }
            }
        }

        private void Push()
        {
            if(AnyConditions() == false) return;
            if(m_Buffer.Count == 0) return;

            for(int i = m_Buffer.Count -1; i >= 0; i--)
            {
                var item = m_Buffer[i];
                if(ExecuteConditions(item) == ExecutionOutcome.Consumed)
                {
                    m_Buffer.RemoveAt(i);

                    // If there's no more condition the stop
                    if(AnyConditions() == false) break;
                }
            }
        }

        private ExecutionOutcome ExecuteConditions(TData data)
        {
            // Do the peek ones first as they don't consume any data
            ExecuteConditions(m_PeekConditions, data);

            return ExecuteConditions(m_WhenConditions, data);
        }

        private ExecutionOutcome ExecuteConditions(IList<ConditionState> conditions, TData data)
        {
            var consumption = ExecutionOutcome.NotConsumed;

            for(int i = conditions.Count -1; i >= 0; i--)
            {
                var (tcs, ifConditon, then) = conditions[i];

                try
                {
                    if(ifConditon(data))
                    {
                        then(data);
                        tcs.SetResult(data);
                        consumption = ExecutionOutcome.Consumed;
                        conditions.RemoveAt(i);
                    }
                }
                catch(Exception e)
                {
                    tcs.SetException(e);
                    conditions.RemoveAt(i);
                }
            }

            return consumption;
        }

        private bool AnyConditions()
        {
            return m_WhenConditions.Count > 0 || m_PeekConditions.Count > 0;
        }

        private Task<TData> Setup(IList<ConditionState> conditions, Func<TData, bool> ifCondition, Action<TData> then)
        {
            if(ifCondition is null) throw new ArgumentNullException(nameof(ifCondition));
            if(then is null) throw new ArgumentNullException(nameof(then));

            var tcs = TaskCompletionSourceEx.CreateAsynchronousCompletionSource<TData>();
            var condition = new ConditionState(tcs, ifCondition, then);

            lock(m_SyncRoot)
            {
                conditions.Add(condition);
                Push();
            }

            return tcs.Task;
        }
    }
}

