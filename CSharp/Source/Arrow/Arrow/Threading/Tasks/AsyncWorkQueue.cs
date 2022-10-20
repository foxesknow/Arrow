using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// TODO: fix tuple
#nullable disable

namespace Arrow.Threading.Tasks
{
    public partial class AsyncWorkQueue : IAsyncWorkQueue, IDisposable
    {
        public static readonly long NoQueue = -1;

        private static long s_NextQueueID = 0;

        private readonly object m_SyncRoot=new object();

        private readonly long m_ID;

        private static readonly ThreadLocal<long> s_ActiveID = new ThreadLocal<long>(() => NoQueue);
		
		private readonly EventWaitHandle m_StopEvent=new AutoResetEvent(false);

        private readonly CustomSynchronizationContext m_SynchronizationContext;
		
		/// <summary>
		/// A list is used instead of a queue as it performs better is our swap/process usage model
		/// </summary>
		private List<IWork> m_PendingWork;
		
		// Although it's slightly faster to allocate a new list in the ProcessQueue
		// method it's better from a GC perspective to keep the list around
		private List<IWork> m_SwapData;
		
		private readonly int m_InitialCapacity;
		
		private readonly IWorkDispatcher m_Dispatcher;
		
		private bool m_ThreadActive;
		
		private bool m_StopProcessing;
		
		private bool m_Disposed;
		
		/// <summary>
		/// Initializes the instance using the default work item dispatcher
		/// </summary>
		public AsyncWorkQueue() : this(null)
		{
		}
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="dispatcher">A dispatcher that will place work into a thread pool. If null the default dispatcher is used</param>
		public AsyncWorkQueue(IWorkDispatcher dispatcher) : this(dispatcher,8)
		{
		}
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="dispatcher">A dispatcher that will place work into a thread pool. If null the default dispatcher is used</param>
		/// <param name="initialCapacity">The initial capacity for the queue</param>
		public AsyncWorkQueue(IWorkDispatcher dispatcher, int initialCapacity)
		{
			if(initialCapacity < 0) throw new ArgumentOutOfRangeException("initialCapacity");
		
			if(dispatcher == null) dispatcher = new ThreadPoolWorkDispatcher();
			
			m_Dispatcher = dispatcher;
			m_InitialCapacity = initialCapacity;
			
			m_PendingWork = new List<IWork>(initialCapacity);
			m_SwapData = new List<IWork>(initialCapacity);

            m_ID = Interlocked.Increment(ref s_NextQueueID);
            m_SynchronizationContext = new CustomSynchronizationContext(this);
		}

        public static long ActiveID
        {
            get{return s_ActiveID.Value;}
        }

        public long ID
        {
            get{return m_ID;}
        }
		
		/// <summary>
		/// Adds a new item to the queue
		/// </summary>
		/// <param name="item">The item to process</param>
		public Task EnqueueAsync(Action action)
		{
            if(action == null) throw new ArgumentNullException(nameof(action));
            
            var work = new ActionWork(action);
            lock(m_SyncRoot)
            {
                DoEnqueue(work);
            }

            return work.Task;
		}

        public (bool Enqueued, Task Task) TryEnqueueAsync(Action action)
		{
            if(action == null) throw new ArgumentNullException(nameof(action));
            
            
            lock(m_SyncRoot)
            {
                if(m_Disposed) return (false, null);

                var work = new ActionWork(action);
                var task = TryDoEnqueue(work);
                return (task != null, task);
            }
		}

        /// <summary>
		/// Adds a new item to the queue
		/// </summary>
		/// <param name="item">The item to process</param>
		public Task EnqueueAsync<T>(T state, Action<T> action)
		{
            if(action == null) throw new ArgumentNullException(nameof(action));
            
            var work = new ActionStateWork<T>(state, action);
            lock(m_SyncRoot)
            {
                DoEnqueue(work);
            }

            return work.Task;
		}

        public (bool Enqueued, Task Task) TryEnqueueAsync<T>(T state, Action<T> action)
		{
            if(action == null) throw new ArgumentNullException(nameof(action));
            
            lock(m_SyncRoot)
            {
                if(m_Disposed) return (false, null);

                var work = new ActionStateWork<T>(state, action);
                var task = TryDoEnqueue(work);
                return (task != null, task);
            }
		}

        /// <summary>
		/// Adds a new item to the queue
		/// </summary>
		/// <param name="item">The item to process</param>
		public Task<TResult> EnqueueAsync<TResult>(Func<TResult> function)
		{
            if(function == null) throw new ArgumentNullException(nameof(function));
            
            var work = new FuncWork<TResult>(function);
            lock(m_SyncRoot)
            {
                DoEnqueue(work);
            }

            return work.Task;
		}

        public (bool Enqueued, Task<TResult> Task) TryEnqueueAsync<TResult>(Func<TResult> function)
		{
            if(function == null) throw new ArgumentNullException(nameof(function));
            
            lock(m_SyncRoot)
            {
                if(m_Disposed) return (false, null);

                var work = new FuncWork<TResult>(function);
                var task = TryDoEnqueue(work);
                return (task != null, task);
            }
		}

        /// <summary>
		/// Adds a new item to the queue
		/// </summary>
		/// <param name="item">The item to process</param>
		public Task<TResult> EnqueueAsync<TState, TResult>(TState state, Func<TState, TResult> function)
		{
            if(function == null) throw new ArgumentNullException(nameof(function));
            
            var work = new FuncStateWork<TState, TResult>(state, function);
            lock(m_SyncRoot)
            {
                DoEnqueue(work);
            }

            return work.Task;
		}

        public (bool Enqueued, Task<TResult> Task) TryEnqueueAsync<TState, TResult>(TState state, Func<TState, TResult> function)
		{
            if(function == null) throw new ArgumentNullException(nameof(function));
            
            lock(m_SyncRoot)
            {
                if(m_Disposed) return (false, null);

                var work = new FuncStateWork<TState, TResult>(state, function);
                var task = TryDoEnqueue(work);
                return (task != null, task);
            }
		}

        /// <summary>
		/// Adds a new item to the queue
		/// </summary>
		/// <param name="item">The item to process</param>
		public Task EnqueueAsync(Func<Task> function)
		{
            if(function == null) throw new ArgumentNullException(nameof(function));
            
            var work = new ProxyFuncWork(function);
            lock(m_SyncRoot)
            {
                DoEnqueue(work);
            }

            return work.Task;
		}

        public (bool Enqueued, Task Task) TryEnqueueAsync(Func<Task> function)
		{
            if(function == null) throw new ArgumentNullException(nameof(function));
            
            lock(m_SyncRoot)
            {
                if(m_Disposed) return (false, null);

                var work = new ProxyFuncWork(function);
                var task = TryDoEnqueue(work);
                return (task != null, task);
            }
		}

        /// <summary>
		/// Adds a new item to the queue
		/// </summary>
		/// <param name="item">The item to process</param>
		public Task EnqueueAsync<TState>(TState state, Func<TState, Task> function)
		{
            if(function == null) throw new ArgumentNullException(nameof(function));
            
            var work = new ProxyFuncStateWork<TState>(state, function);
            lock(m_SyncRoot)
            {
                DoEnqueue(work);
            }

            return work.Task;
		}

        public (bool Enqueued, Task Task) TryEnqueueAsync<TState>(TState state, Func<TState, Task> function)
		{
            if(function == null) throw new ArgumentNullException(nameof(function));
            
            lock(m_SyncRoot)
            {
                if(m_Disposed) return (false, null);

                var work = new ProxyFuncStateWork<TState>(state, function);
                var task = TryDoEnqueue(work);
                return (task != null, task);
            }
		}

        /// <summary>
		/// Adds a new item to the queue
		/// </summary>
		/// <param name="item">The item to process</param>
		public Task<TResult> EnqueueAsync<TResult>(Func<Task<TResult>> function)
		{
            if(function == null) throw new ArgumentNullException(nameof(function));
            
            var work = new ProxyFuncWork<TResult>(function);
            lock(m_SyncRoot)
            {
                DoEnqueue(work);
            }

            return work.Task;
		}

        public (bool Enqueued, Task<TResult> Task) TryEnqueueAsync<TResult>(Func<Task<TResult>> function)
		{
            if(function == null) throw new ArgumentNullException(nameof(function));
            
            lock(m_SyncRoot)
            {
                if(m_Disposed) return (false, null);

                var work = new ProxyFuncWork<TResult>(function);
                var task = TryDoEnqueue(work);
                return (task != null, task);
            }
		}

        /// <summary>
		/// Adds a new item to the queue
		/// </summary>
		/// <param name="item">The item to process</param>
		public Task<TResult> EnqueueAsync<TState, TResult>(TState state, Func<TState, Task<TResult>> function)
		{
            if(function == null) throw new ArgumentNullException(nameof(function));
            
            var work = new ProxyFuncStateWork<TState, TResult>(state, function);
            lock(m_SyncRoot)
            {
                DoEnqueue(work);
            }

            return work.Task;
		}

        public (bool Enqueued, Task<TResult> Task) TryEnqueueAsync<TState, TResult>(TState state, Func<TState, Task<TResult>> function)
		{
            if(function == null) throw new ArgumentNullException(nameof(function));
            
            lock(m_SyncRoot)
            {
                if(m_Disposed) return (false, null);

                var work = new ProxyFuncStateWork<TState, TResult>(state, function);
                var task = TryDoEnqueue(work);
                return (task != null, task);
            }
		}

        private void ContextEnqueueAsync(SendOrPostCallback callback, object data)
        {
            var work = new SendOrPostCallbackWork(callback, data);

            lock(m_SyncRoot)
            {
                DoEnqueue(work);
            }
        }

		private void DoEnqueue(IWork item)
		{
			if(m_Disposed) throw new ObjectDisposedException(nameof(AsyncWorkQueue));
				
			m_PendingWork.Add(item);
				
			if(m_ThreadActive == false)
			{
				m_ThreadActive = true;
				m_Dispatcher.QueueUserWorkItem(ProcessQueue);
			}
		}

        private Task<T> TryDoEnqueue<T>(WorkBase<T> workBase)
        {
            if(m_Disposed) return null;
            DoEnqueue(workBase);

            return workBase.Task;
        }
		
		/// <summary>
		/// Returns the work item dispatcher being used to schedule threads
		/// </summary>
		public IWorkDispatcher WorkItemDispatcher
		{
			get{return m_Dispatcher;}
		}
		
		/// <summary>
		/// Returns the number of items in the queue
		/// </summary>
		public int Count
		{
			get
			{
				lock(m_SyncRoot)
				{
					return m_PendingWork.Count;
				}
			}
		}
		
		/// <summary>
		/// Stops processing and releases any resources
		/// </summary>
		public void Dispose()
		{
			bool shouldWait = false;
			
			lock(m_SyncRoot)
			{
				if(m_Disposed) return;
			
				m_StopProcessing = true;
				m_Disposed = true;
				
				// We only need to block if the thread is currently running
				if(m_ThreadActive) shouldWait = true;
			}		
			
			if(shouldWait) m_StopEvent.WaitOne();
			
			m_StopEvent.Close();
		}
		
		/// <summary>
		/// Removes any pending work items from the work queue
		/// </summary>
		public void Clear()
		{
			lock(m_SyncRoot)
			{
				m_PendingWork.Clear();
			}
		}
		
		/// <summary>
		/// Waits for any outstanding work to be processed and shuts the queue down
		/// </summary>
		public void Close()
		{
			Dispose();
		}
		
		/// <summary>
		/// Switches the pending work queue with an empty queue
		/// </summary>
		/// <returns>The work that should be run next</returns>
		private List<IWork> SwitchData()
		{
			var workToProcess=m_PendingWork;
			m_PendingWork=m_SwapData;
			m_SwapData=workToProcess;

			return workToProcess;
		}

		/// <summary>
		/// Processes all the items in the queue
		/// </summary>
		/// <param name="state">Not used</param>
		private void ProcessQueue(object state)
		{
			lock(m_SyncRoot)
			{
                var previousContext = SynchronizationContext.Current;
                SynchronizationContext.SetSynchronizationContext(m_SynchronizationContext);
                s_ActiveID.Value = m_ID;

				try
				{
					while(m_PendingWork.Count!=0 && m_StopProcessing==false)
					{
						var workToProcess=SwitchData();
					
						// We can release the lock here
						Monitor.Exit(m_SyncRoot);
						
						try
						{
							ProcessItems(workToProcess);
						}
						finally
						{					
							// We need to remove the data from the swap
							workToProcess.Clear();							
							
							// And re-acquire it here before we loop back around
							// We do it in a finally block to remain consistent
							// if a ThreadAbortException is thrown
							Monitor.Enter(m_SyncRoot);
						}
					}
				}
				finally
				{				
					// When we're about to leave we reset the active state so that a new thread can be launched
					// This is done in a finally block in case we get a ThreadAbortException exception
					m_ThreadActive=false;
					
					if(m_StopProcessing) 
					{
						// Process anything that's left
						ProcessItems(m_PendingWork);
						
						m_StopEvent.Set();
					}

                    SynchronizationContext.SetSynchronizationContext(previousContext);
                    s_ActiveID.Value = NoQueue;
				}
			}
		}
		
		private void ProcessItems(List<IWork> items)
		{
			for(int i=0; i<items.Count; i++)
			{
				var item = items[i];
				item.Execute();
			}
		}       
	

        interface IWork
        {
            void Execute();
        }

        class SendOrPostCallbackWork : IWork
        {
            private readonly SendOrPostCallback m_Callback;
            private readonly object m_Data;

            public SendOrPostCallbackWork(SendOrPostCallback callback, object data)
            {
                this.m_Callback = callback;
                this.m_Data = data;
            }

            public void Execute()
            {
                m_Callback(m_Data);
            }
        }

        abstract class WorkBase<T> : IWork
        {
            public Task<T> Task
            {
                get{return TaskCompletionSource.Task;}
            }

            protected TaskCompletionSource<T> TaskCompletionSource{get;} = TaskCompletionSourceEx.CreateAsynchronousCompletionSource<T>();

            protected abstract void Run();

            public void Execute()
            {
                try
                {
                    Run();
                }
                catch(Exception e)
                {
                    this.TaskCompletionSource.SetException(e);
                }
            }
        }

        class ActionWork : WorkBase<bool>
        {
            private readonly Action m_Action;

            public ActionWork(Action action)
            {
                m_Action = action;
            }

            protected override void Run()
            {
                m_Action();
                this.TaskCompletionSource.SetResult(true);
            }
        }

        class ActionStateWork<TState> : WorkBase<bool>
        {
            private readonly TState m_State;
            private readonly Action<TState> m_Action;

            public ActionStateWork(TState state, Action<TState> action)
            {
                m_State = state;
                m_Action = action;
            }

            protected override void Run()
            {
                m_Action(m_State);
                this.TaskCompletionSource.SetResult(true);
            }
        }

        class FuncWork<T> : WorkBase<T>
        {
            private readonly Func<T> m_Function;

            public FuncWork(Func<T> function)
            {
                m_Function = function;
            }

            protected override void Run()
            {
                var result = m_Function();
                this.TaskCompletionSource.SetResult(result);
            }
        }

        class FuncStateWork<TState, TResult> : WorkBase<TResult>
        {
            private readonly TState m_State;
            private readonly Func<TState, TResult> m_Function;

            public FuncStateWork(TState state, Func<TState, TResult> function)
            {
                m_State = state;
                m_Function = function;
            }

            protected override void Run()
            {
                var result = m_Function(m_State);
                this.TaskCompletionSource.SetResult(result);
            }
        }

        class ProxyFuncWork : WorkBase<bool>
        {
            private readonly Func<Task> m_Function;

            public ProxyFuncWork(Func<Task> function)
            {
                m_Function = function;
            }

            protected override async void Run()
            {
                await m_Function();
                this.TaskCompletionSource.SetResult(true);
            }
        }

        class ProxyFuncWork<TResult> : WorkBase<TResult>
        {
            private readonly Func<Task<TResult>> m_Function;

            public ProxyFuncWork(Func<Task<TResult>> function)
            {
                m_Function = function;
            }

            protected override async void Run()
            {
                var result = await m_Function();
                this.TaskCompletionSource.SetResult(result);
            }
        }

        class ProxyFuncStateWork<TState> : WorkBase<bool>
        {
            private readonly TState m_State;
            private readonly Func<TState, Task> m_Function;

            public ProxyFuncStateWork(TState state, Func<TState, Task> function)
            {
                m_State = state;
                m_Function = function;
            }

            protected override async void Run()
            {
                await m_Function(m_State);
                this.TaskCompletionSource.SetResult(true);
            }
        }

        class ProxyFuncStateWork<TState, TResult> : WorkBase<TResult>
        {
            private readonly TState m_State;
            private readonly Func<TState, Task<TResult>> m_Function;

            public ProxyFuncStateWork(TState state, Func<TState, Task<TResult>> function)
            {
                m_State = state;
                m_Function = function;
            }

            protected override async void Run()
            {
                var result = await m_Function(m_State);
                this.TaskCompletionSource.SetResult(result);
            }
        }

    }
}
