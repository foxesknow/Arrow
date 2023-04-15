using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Arrow.Execution;

namespace Arrow.Calendar
{
    /// <summary>
    /// Schedules one-shot reminders for a future date.
    /// The reminders are not guaranteed to run on any particular thread
    /// 
    /// The Clock class is used to determine the current time.
    /// If you alter/replace the clock then you can call
    /// Reschedule to take any time changes into account.
    /// 
    /// NOTE: If 2 or more jobs are scheduled to run at the same time then
    /// they are run in the order they werre registered.
    /// </summary>
    public sealed partial class Reminders : IReminders, IDisposable
    {
        private static readonly IComparer<Job> s_JobComparer = new JobComparer();

        private readonly object m_SyncRoot = new();

        private Timer? m_Timer;
        private readonly List<Job> m_Jobs = new();
        
        private readonly Action<object?> m_CallViaState;


        /// <summary>
        /// Raised after the timer has fired
        /// </summary>
        public event EventHandler<RemindersNotifiedEventArgs>? RaisedReminders;

        /// <summary>
        /// Initializes the instance
        /// </summary>
        public Reminders()
        {
            m_CallViaState = CallViaState;
            m_Timer = new Timer(HandleTimer);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            lock(m_SyncRoot)
            {
                if(m_Timer is not null)
                {
                    m_Timer.Dispose();
                    m_Jobs.Clear();
                    m_Timer = null; 
                }
            }
        }

        /// <summary>
        /// This method is for testing
        /// </summary>
        internal IReadOnlyList<ReminderID> GetRemindersIDs()
        {
            lock(m_SyncRoot)
            {
                return m_Jobs.Select(job => job.ID).ToArray();
            }
        }

        /// <summary>
        /// Returns the number of pending reminders
        /// </summary>
        public int Count
        {
            get
            {
                lock(m_SyncRoot)
                {
                    return m_Jobs.Count;
                }
            }
        }

        /// <inheritdoc>
        public ReminderID Add(DateTime when, Action reminder)
        {
            if(reminder is null) throw new ArgumentNullException(nameof(reminder));

            return Add(when, reminder, CallViaState);
        }

        /// <inheritdoc>
        public ReminderID Add(DateTime when, object? state, Action<object?> reminder)
        {
            if(reminder is null) throw new ArgumentNullException(nameof(reminder));

            var id = ReminderID.Allocate();
            when = Normalize(when);

            lock(m_SyncRoot)
            {
                var changedSchedule = false;

                if(m_Jobs.Count == 0)
                {
                    changedSchedule = true;
                }
                else if(when < m_Jobs[m_Jobs.Count - 1].When)
                {
                    // The new job is the new earliest to run
                    changedSchedule = true;
                }

                
                m_Jobs.Add(new(id, when, state, reminder));
                SortJobs();

                if(changedSchedule) ScheduleTimer();
            }

            return id;
        }

        /// <inheritdoc>
        public bool Cancel(ReminderID id)
        {
            if(id == ReminderID.None) return false;

            lock(m_SyncRoot)
            {
                var index = -1;

                // NOTE: Loop rather than Linq to avoid a closure
                for(var i = 0; i < m_Jobs.Count; i++)
                {
                    if(m_Jobs[i].ID == id)
                    {
                        index = i;
                        break;
                    }
                }

                if(index == -1) return false;

                // We'll only need to reschedule if we're removing the next job
                // to run, which is at the end of the list
                var needToReschedule = (index == m_Jobs.Count - 1);

                m_Jobs.RemoveAt(index);
                if(needToReschedule) ScheduleTimer();
            }

            return true;
        }

        /// <summary>
        /// Reschedules any reminders due to an external change in the clock
        /// </summary>
        public void Reschedule()
        {
            RescheduleOutcome outcome;

            lock(m_SyncRoot)
            {
                outcome = ScheduleTimer();
            }

            if(outcome == RescheduleOutcome.RunScheduled)
            {
                // There are jobs to be run in the future.
                // Let any subscribers know that nothing was done as part of the reschedule.
                RaiseRaisedReminders(0);
            }
        }

        /// <summary>
        /// Removes all reminders
        /// </summary>
        public void Clear()
        {
            lock(m_SyncRoot)
            {
                m_Jobs.Clear();
                ScheduleTimer();
            }
        }

        /// <summary>
        /// Works out when the timer should next fire
        /// </summary>
        /// <returns></returns>
        private RescheduleOutcome ScheduleTimer()
        {
            if(m_Timer is null) return RescheduleOutcome.NotRunning;

            if(m_Jobs.Count == 0)
            {
                m_Timer.Change(Timeout.Infinite, Timeout.Infinite);
                return RescheduleOutcome.NothingToRun;
            }

            var outcome = RescheduleOutcome.RunScheduled;
            var now = Clock.UtcNow;
            var next = m_Jobs[m_Jobs.Count - 1].When;
            long dueTime = (long)(next - now).TotalMilliseconds;

            if(dueTime <= 0)
            {
                // The next item to run should have happened in the past!
                // We'll schedule to run immediately
                dueTime = 0;
                outcome = RescheduleOutcome.RanImmediate;
            }

            m_Timer.Change(dueTime, Timeout.Infinite);
            
            return outcome;
        }

        private void HandleTimer(object? state)
        {
            List<Job>? tasksToRun = null;

            var now = Clock.UtcNow;

            // We run the jobs outside of the lock to allow the
            // reminders to call back in without having to worry about threading issues.
            lock(m_SyncRoot)
            {
                // The jobs are sorted so that the next job to run is at the end of the list
                while(m_Jobs.Count != 0)
                {
                    var nextJobIndex = m_Jobs.Count - 1;

                    var job = m_Jobs[nextJobIndex];
                    if(job.When > now) break;

                    if(tasksToRun is null) tasksToRun = new();
                    tasksToRun.Add(job);

                    m_Jobs.RemoveAt(nextJobIndex);
                }
            }

            if(tasksToRun is not null)
            {
                for(var i = 0; i < tasksToRun.Count; i++)
                {
                    MethodCall.AllowFail(tasksToRun[i], static job => job.Reminder(job.State));
                }

                RaiseRaisedReminders(tasksToRun.Count);
            }

            lock(m_SyncRoot)
            {
                ScheduleTimer();
            }
        }

        private void RaiseRaisedReminders(int jobCount)
        {
            var callback = RaisedReminders;
            if(callback is not null)
            {
                MethodCall.AllowFail((Self: this, jobCount, callback), static state =>
                {
                    state.callback(state.Self, new RemindersNotifiedEventArgs(state.jobCount));
                });
            }
        }

        /// <summary>
        /// Converts the date to UTC, which is what we'll use for scheduling
        /// </summary>
        /// <param name="when"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private DateTime Normalize(DateTime when)
        {
            var normalizedWhen = when.ToUniversalTime();
            if(normalizedWhen.Kind != DateTimeKind.Utc) throw new ArgumentException("when cannot be converted to UTC");

            return normalizedWhen;
        }

        /// <summary>
        /// Sorts the jobs from latest to earliest.
        /// This means when we remove jobs that of run we'll do so from the end of the
        /// array which doesn't requiring moving items to fill the gaps.
        /// </summary>
        private void SortJobs()
        {
            // We could have used the delegate version of sort, but it wraps the delegate
            // in a IComparer<> which causes a memory allocation.
            m_Jobs.Sort(s_JobComparer);
        }

        /// <summary>
        /// Calls the action passed in via the state parameter.
        /// This is used to call Action instances that don't have any state data.
        /// </summary>
        /// <param name="state"></param>
        private void CallViaState(object? state)
        {
            var action = (Action)state!;
            action();
        }

        private sealed class JobComparer : IComparer<Job>
        {
            int IComparer<Job>.Compare(Job lhs, Job rhs)
            {
                // Sort the jobs so that the latest job goes to the front and the earliest goes to the end.
                // This will make it easier to remove then when we run the timer as we'll just be taking
                // from the end and avoiding copying bits of the underling array
                // We'll also sort by ID so that if 2 jobs have the same When then the first to be registered is run.
                var result = rhs.When.CompareTo(lhs.When);

                if(result == 0)
                {
                    result = rhs.ID.CompareTo(lhs.ID);
                }

                return result;
            }
        }


        enum RescheduleOutcome
        {
            NotRunning,
            NothingToRun,
            RanImmediate,
            RunScheduled
        }
    }
}
