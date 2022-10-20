using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Arrow.Execution;

namespace Arrow.Calendar
{
	/// <summary>
	/// Schedules one-shot reminders for a future date.
	/// Reminders are not guaranteed to run on any particular thread
	/// </summary>
	public class Reminders : IDisposable
	{
		private readonly object m_SyncRoot=new object();
		
		private Timer? m_Timer;
		
		private readonly List<ScheduledAction> m_ScheduledActions=new List<ScheduledAction>();
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		public Reminders()
		{
			m_Timer=new Timer(HandlerTimer);
		}
		
		/// <summary>
		/// Adds a new reminder.
		/// If the time to run the reminder is in the past then the reminder will trigger immediately
		/// </summary>
		/// <param name="when">When to perform the action. The time should be convertible to utc</param>
		/// <param name="reminder">What the action is</param>
		/// <exception cref="System.ArgumentNullException">reminder is null</exception>
		public void Add(DateTime when, Action reminder)
		{
			if(reminder==null) throw new ArgumentNullException("reminder");
			
			when=MassageDateTime(when);
			
			lock(m_SyncRoot)
			{
				bool changeSchedule=false;
				
				if(m_ScheduledActions.Count==0)
				{
					// The first item all requires a schedule change
					changeSchedule=true;
				}
				else if(when<m_ScheduledActions[0].When)
				{
					changeSchedule=true;
				}
				
				m_ScheduledActions.Add(new ScheduledAction(when,reminder));
				SortTasks();
				
				if(changeSchedule) ScheduleTimer();
			}			
		}
		
		/// <summary>
		/// Removes a reminder
		/// </summary>
		/// <param name="reminder">The reminder action</param>
		/// <returns>true if the action was found and removed, otherwise false</returns>
		/// <exception cref="System.ArgumentNullException">reminder is null</exception>
		public bool Remove(Action reminder)
		{
			if(reminder==null) throw new ArgumentNullException("reminder");
			
			bool removed=false;
			
			lock(m_SyncRoot)
			{
				int index=m_ScheduledActions.FindIndex(task=>task.Reminder==reminder);
				
				if(index!=-1)
				{
					m_ScheduledActions.RemoveAt(index);
					removed=true;
					
					if(index==0) ScheduleTimer();
				}
			}
			
			return removed;
		}
		
		/// <summary>
		/// Removes all instances of 
		/// </summary>
		/// <param name="reminder">The reminder to remove</param>
		/// <returns>The number of reminders removed</returns>
		/// <exception cref="System.ArgumentNullException">reminder is null</exception>
		public int RemoveAll(Action reminder)
		{
			if(reminder==null) throw new ArgumentNullException("reminder");
			
			lock(m_SyncRoot)
			{
				int removed=m_ScheduledActions.RemoveAll(task=>task.Reminder==reminder);
				
				if(removed!=0) ScheduleTimer();
				
				return removed;
			}
		}
		
		/// <summary>
		/// Checks to see if a reminder is present
		/// </summary>
		/// <param name="reminder">The reminder to check for</param>
		/// <returns>true if the reminder is present, false otherwise</returns>
		/// <exception cref="System.ArgumentNullException">reminder is null</exception>
		public bool Contains(Action reminder)
		{
			if(reminder==null) throw new ArgumentNullException("reminder");
		
			lock(m_SyncRoot)
			{
				int index=m_ScheduledActions.FindIndex(task=>task.Reminder==reminder);
				return index!=-1;
			}
		}
		
		/// <summary>
		/// Removes all reminders
		/// </summary>
		public void Clear()
		{
			lock(m_SyncRoot)
			{
				m_ScheduledActions.Clear();
				ScheduleTimer();
			}
		}
		
		/// <summary>
		/// Returns the number of reminders
		/// </summary>
		public int Count
		{
			get
			{
				lock(m_SyncRoot)
				{
					return m_ScheduledActions.Count;
				}
			}
		}
		
		/// <summary>
		/// Works out when the next timer event should occur
		/// </summary>
		private void ScheduleTimer()
		{
			if(m_Timer==null) return;
		
			if(m_ScheduledActions.Count==0)
			{
				m_Timer.Change(Timeout.Infinite,Timeout.Infinite);
			}
			else
			{
				DateTime now=Clock.UtcNow;
				DateTime when=m_ScheduledActions[0].When;
				long dueTime=(long)(when-now).TotalMilliseconds;
				
				// If the due time is negative it means the next time
				// is a time in the past. In this case we'll schedule
				// the timer to trigger immediately
				if(dueTime<0) dueTime=0;
				m_Timer.Change(dueTime,Timeout.Infinite);
			}
		}
		
		/// <summary>
		/// Sorts the tasks
		/// </summary>
		private void SortTasks()
		{
			// Sort the tasks so the smallest comes first
			m_ScheduledActions.Sort((lhs,rhs)=>lhs.When.CompareTo(rhs.When));
		}
		
		/// <summary>
		/// Massages a date into the correct format.
		/// Dates must be in UTC format, or be convertible to UTC
		/// </summary>
		/// <param name="when"></param>
		/// <returns></returns>
		private DateTime MassageDateTime(DateTime when)
		{
			when=when.ToUniversalTime();			
			if(when.Kind!=DateTimeKind.Utc) throw new ArgumentException("when cannot be converted to UTC");
						
			return when;
		}
		
		/// <summary>
		/// Called when the timer triggers
		/// </summary>
		/// <param name="state"></param>
		private void HandlerTimer(object? state)
		{
			// We'll run the tasks outside of the sync root
			// so that the actions can call back into the 
			// Reminder instance to re-register without having 
			// to worry about thread issues
			var tasksToRun=new List<ScheduledAction>();
		
			lock(m_SyncRoot)
			{
				DateTime now=Clock.UtcNow;
				
				for(int i=0; i<m_ScheduledActions.Count; i++)
				{
					ScheduledAction task=m_ScheduledActions[i];
					if(task.When>now) break;
					
					tasksToRun.Add(task);
				}
				
				m_ScheduledActions.RemoveRange(0,tasksToRun.Count);
			}
			
			foreach(ScheduledAction task in tasksToRun)
			{
				MethodCall.AllowFail(task.Reminder);
			}
			
			lock(m_SyncRoot)
			{
				ScheduleTimer();
			}
		}

		/// <summary>
		/// Stops scheduling reminders and releases any resources
		/// </summary>
		public void Dispose()
		{
			lock(m_SyncRoot)
			{
				if(m_Timer!=null)
				{
					m_Timer.Dispose();
					m_Timer=null;
					m_ScheduledActions.Clear();
				}
			}
		}
		
		class ScheduledAction  : IEquatable<ScheduledAction>
		{
			public ScheduledAction(DateTime when, Action reminder)
			{
				this.When=when;
				this.Reminder=reminder;
			}
		
			public DateTime When{get;private set;}
			public Action Reminder{get;private set;}

			public override bool Equals(object? obj)
			{
				var rhs=obj as ScheduledAction;
				if(rhs==null) return false;
				
				return Equals(rhs);
			}

			public override int GetHashCode()
			{
				return this.When.GetHashCode();
			}

			public bool Equals(ScheduledAction? other)
			{
				return other is not null && this.When==other.When;
			}
		}
	}
}
