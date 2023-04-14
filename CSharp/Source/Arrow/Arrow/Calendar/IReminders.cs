using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Calendar
{
    /// <summary>
    /// Defines the behaviours of a reminders implementation
    /// </summary>
    public interface IReminders
    {
        /// <summary>
        /// Adds a new reminder
        /// </summary>
        /// <param name="when"></param>
        /// <param name="reminder"></param>
        /// <returns>A unique identifier for the reminder</returns>
        public ReminderID Add(DateTime when, Action reminder);

        /// <summary>
        /// Adds a new reminder
        /// </summary>
        /// <param name="when"></param>
        /// <param name="state">Any additional state to pass to the reminder</param>
        /// <param name="reminder"></param>
        /// <returns>A unique identifier for the reminder</returns>
        public ReminderID Add(DateTime when, object? state, Action<object?> reminder);

        /// <summary>
        /// Attempts to cancel a reminder
        /// </summary>
        /// <param name="id">The ID of the reminder to cancel</param>
        /// <returns>true if the reminder was cancelled, otherwise false</returns>
        public bool Cancel(ReminderID id);
    }
}
