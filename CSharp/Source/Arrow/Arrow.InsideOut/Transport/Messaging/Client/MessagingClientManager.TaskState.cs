using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.InsideOut.Transport.Messaging.Client;

public partial class MessagingClientManager
{
    private abstract class TaskState
    {
        public abstract void TrySetException(Exception exception);
    }

    private sealed class TaskState<T> : TaskState
    {
        public TaskCompletionSource<T> Source{get;} = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public override void TrySetException(Exception exception)
        {
            this.Source.TrySetException(exception);
        }
    }
}
