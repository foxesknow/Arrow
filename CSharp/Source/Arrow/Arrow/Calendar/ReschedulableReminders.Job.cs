using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Calendar
{
    public partial class ReschedulableReminders
    {
        private readonly struct Job : IEquatable<Job>
        {
            public Job(DateTime when, object? state, Action<object?> reminder)
            {
                this.When = when;
                this.State = state;
                this.Reminder = reminder;                
            }

            public DateTime When{get;}
            
            public Action<object?> Reminder{get;}

            public object? State{get;}

            public bool Equals(Job other)
            {
                return this.When == other.When;
            }

            public override bool Equals(object? obj)
            {
                return obj is Job job && Equals(job);
            }

            public override int GetHashCode()
            {
                return this.When.GetHashCode();
            }

            public override string ToString()
            {
                return this.When.ToString();
            }
        }
    }
}
