using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Threading.Tasks
{
    public sealed partial class Signaller<T>
    {
        sealed class State
        {
            public State(long id, Func<T, bool> condition)
            {
                this.ID = id;
                this.Condition = condition;
                this.Task = this.Tcs.Task;
            }

            public readonly TaskCompletionSource<T> Tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);

            public readonly long ID;
            public readonly Func<T, bool> Condition;
            public readonly Task<T> Task;
        }

        /// <summary>
        /// This token is used by classes to build higher level signal constructs
        /// </summary>
        public readonly struct WhenToken
        {
            public WhenToken(Task<T> task, long id)
            {
                this.Task = task;
                this.ID = id;
            }

            public Task<T> Task{get;}

            public long ID{get;}
        }
    }
}
