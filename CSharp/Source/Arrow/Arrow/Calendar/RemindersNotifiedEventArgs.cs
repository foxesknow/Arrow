using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Calendar
{
    public sealed class RemindersNotifiedEventArgs : EventArgs
    {
        public RemindersNotifiedEventArgs(int remindersNotified)
        {
            this.RemindersNotified = remindersNotified;
        }


        public int RemindersNotified{get;}
    }
}
