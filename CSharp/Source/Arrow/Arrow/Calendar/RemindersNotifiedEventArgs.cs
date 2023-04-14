using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Calendar
{
    /// <summary>
    /// Holds information on how many reminders were notified
    /// </summary>
    public sealed class RemindersNotifiedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="remindersNotified"></param>
        public RemindersNotifiedEventArgs(int remindersNotified)
        {
            this.RemindersNotified = remindersNotified;
        }
        
        /// <summary>
        ///  How many reminders for notified
        /// </summary>
        public int RemindersNotified{get;}
    }
}
