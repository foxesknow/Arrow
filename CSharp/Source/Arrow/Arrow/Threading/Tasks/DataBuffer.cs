using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Arrow.Execution;

namespace Arrow.Threading.Tasks
{
    public partial class DataBuffer<TData>
    {
        private readonly object m_SyncRoot = new object();

        private readonly List<TData> m_Buffer = new();
        private readonly List<ConditionState> m_WhenConditions = new();
        private readonly List<ConditionState> m_PeekConditions = new();

        public DataBuffer() : this(DataTimeout.WaitForever)
        {
        }

        public DataBuffer(TimeSpan waitTimeout)
        {
            this.DefaultTimeout = waitTimeout;
        }

        public TimeSpan DefaultTimeout{get;}

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

        public void Clear()
        {
            lock(m_SyncRoot)
            {
                m_Buffer.Clear();
            }
        }

        public void Reset()
        {
            lock(m_SyncRoot)
            {
                Clear();
                CancelAll();
            }
        }

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

        public IDisposable StartActivity()
        {
            Reset();
            return new Disposer(() => Reset());
        }

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

        public Task<TData> WaitFor(TimeSpan timeout, Func<TData, bool> ifCondition, Action<TData> then)
        {
            return MakeTask(m_WhenConditions, timeout, ifCondition, then);
        }

        public Task<TData> PeekFor(TimeSpan timeout, Func<TData, bool> ifCondition, Action<TData> then)
        {
            return MakeTask(m_PeekConditions, timeout, ifCondition, then);
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

        public bool TryPeek(out TData data, Func<TData, bool> condition)
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

            var tcs = new TaskCompletionSource<TData>(TaskCreationOptions.RunContinuationsAsynchronously);
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

