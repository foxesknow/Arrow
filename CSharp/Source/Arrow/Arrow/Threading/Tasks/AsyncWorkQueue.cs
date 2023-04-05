using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.Threading.Tasks
{
	/// <summary>
	/// A work queue that supports asynchronous methods.
	/// 
	/// The queue implements its own synchronization context to ensure that when a scheduled function
	/// resumes after an await it comes back onto this queu, given queue-affinity to anything run on
	/// this queue.
	/// </summary>
    public sealed partial class AsyncWorkQueue : IDisposable
    {
        public static readonly long NoActiveQueue = 0;

        private static long s_NextQueueID = 0;
        private static readonly ThreadLocal<long> s_ActiveID = new ThreadLocal<long>(() => NoActiveQueue);

        private readonly object m_SyncRoot=new object();

        private readonly long m_ID;        
		
		private readonly EventWaitHandle m_StopEvent=new AutoResetEvent(false);

        private readonly CustomSynchronizationContext m_QueueContext;
		
		/// <summary>
		/// A list is used instead of a queue as it performs better is our swap/process usage model
		/// </summary>
		private List<StateData> m_Data;
		
		// Although it's slightly faster to allocate a new list in the ProcessQueue
		// method it's better from a GC perspective to keep the list around
		private List<StateData> m_SwapData;
		
		private readonly int m_InitialCapacity;
		
		private readonly IWorkDispatcher m_Dispatcher;
		
		private bool m_ThreadActive;
		
		private bool m_StopProcessing;
		
		private bool m_Disposed;

        private readonly WaitCallback m_ProcessQueue;

		public EventHandler<UnhandledAsyncWorkQueueExceptionEventArgs>? UnhandledExceptionOnSynchronizationContext;
		
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
		public AsyncWorkQueue(IWorkDispatcher? dispatcher) : this(dispatcher,8)
		{
		}
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="dispatcher">A dispatcher that will place work into a thread pool. If null the default dispatcher is used</param>
		/// <param name="initialCapacity">The initial capacity for the queue</param>
		public AsyncWorkQueue(IWorkDispatcher? dispatcher, int initialCapacity)
		{
			if(initialCapacity < 0) throw new ArgumentOutOfRangeException("initialCapacity");
		
			if(dispatcher == null) dispatcher = new ThreadPoolWorkDispatcher();
			
			m_Dispatcher = dispatcher;
			m_InitialCapacity = initialCapacity;
			
			m_Data = new(initialCapacity);
			m_SwapData = new(initialCapacity);

            m_ID = Interlocked.Increment(ref s_NextQueueID);
            m_QueueContext = new CustomSynchronizationContext(this);
            m_ProcessQueue = ProcessQueue;
		}

		public void Dispose()
		{
			var shouldWait = false;

			lock(m_SyncRoot)
			{
				if(m_Disposed) return;

				m_StopProcessing = true;
				m_Disposed = true;

				if(m_ThreadActive) shouldWait = true;
			}

			if(shouldWait) m_StopEvent.WaitOne();

			m_StopEvent.Close();
		}

		public void Close()
		{
			Dispose();
		}

        public static long ActiveID
        {
            get{return s_ActiveID.Value;}
        }

        public long ID
        {
            get{return m_ID;}
        }

		private bool IsDisposed
		{
			get{return m_Disposed;}
		}

		/// <summary>
		/// Adds a new item to the queue.
		/// The function will run asynchronously but the caller will not be able to
		/// get information back. Any exceptions are discarded
		/// </summary>
		/// <typeparam name="TState"></typeparam>
		/// <param name="state"></param>
		/// <param name="function"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public void Enqueue<TState>(TState state, Func<TState, Task> function)
		{
			if(function is null) throw new ArgumentNullException(nameof(function));

			var stateData = new NoTaskFunctionStateData<TState>(state, function);

			lock(m_SyncRoot)
			{
				Schedule(stateData);
			}
		}

		/// <summary>
		/// Attempts to adds a new item to the queue.
		/// The function will run asynchronously but the caller will not be able to
		/// get information back. Any exceptions are discarded
		/// </summary>
		/// <typeparam name="TState"></typeparam>
		/// <param name="state"></param>
		/// <param name="function"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public bool TryEnqueue<TState>(TState state, Func<TState, Task> function)
		{
			if(function is null) throw new ArgumentNullException(nameof(function));			

			lock(m_SyncRoot)
			{
				if(m_Disposed) return false;

				var stateData = new NoTaskFunctionStateData<TState>(state, function);
				Schedule(stateData);

				return true;
			}
		}

		/// <summary>
		/// Adds an action to the queue
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public Task EnqueueAsync(Action action)
		{
			if(action is null) throw new ArgumentNullException(nameof(action));

			var stateData = new ActionStateData(action);

			lock(m_SyncRoot)
			{
				Schedule(stateData);
				return stateData.Task;
			}
		}

		/// <summary>
		/// Attempts to enqueue an action
		/// </summary>
		/// <param name="action"></param>
		/// <param name="task"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public bool TryEnqueueAsync(Action action, [NotNullWhen(true)] out Task? task)
		{
			if(action is null) throw new ArgumentNullException(nameof(action));

			lock(m_SyncRoot)
			{
				if(m_Disposed)
				{
					task = null;
					return false;
				}

				var stateData = new ActionStateData(action);
				Schedule(stateData);
				
				task = stateData.Task;
				return true;
			}
		}

		/// <summary>
		/// Adds a new action to the queue
		/// </summary>
		/// <typeparam name="TState"></typeparam>
		/// <param name="state"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public Task EnqueueAsync<TState>(TState state, Action<TState> action)
		{
			if(action is null) throw new ArgumentNullException(nameof(action));

			var stateData = new ActionStateData<TState>(state, action);

			lock(m_SyncRoot)
			{
				Schedule(stateData);
				return stateData.Task;
			}
		}

		/// <summary>
		/// Attempts to enqueue an action
		/// </summary>
		/// <typeparam name="TState"></typeparam>
		/// <param name="state"></param>
		/// <param name="action"></param>
		/// <param name="task"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public bool TryEnqueueAsync<TState>(TState state, Action<TState> action, [NotNullWhen(true)] out Task? task)
		{
			if(action is null) throw new ArgumentNullException(nameof(action));

			lock(m_SyncRoot)
			{
				if(m_Disposed)
				{
					task = null;
					return false;
				}

				var stateData = new ActionStateData<TState>(state, action);
				Schedule(stateData);
				
				task = stateData.Task;
				return true;
			}
		}

		/// <summary>
		/// Adds a new function to the queue
		/// </summary>
		/// <param name="function"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public Task EnqueueAsync(Func<Task> function)
		{
			if(function is null) throw new ArgumentNullException(nameof(function));

			var stateData = new FuncStateData(function);

			lock(m_SyncRoot)
			{
				Schedule(stateData);
				return stateData.Task;
			}
		}

		/// <summary>
		/// Attempts to enqueue a function
		/// </summary>
		/// <param name="function"></param>
		/// <param name="task"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public bool TryEnqueueAsync(Func<Task> function, [NotNullWhen(true)] out Task? task)
		{
			if(function is null) throw new ArgumentNullException(nameof(function));

			lock(m_SyncRoot)
			{
				if(m_Disposed)
				{
					task = null;
					return false;
				}
				
				var stateData = new FuncStateData(function);
				Schedule(stateData);
				
				task = stateData.Task;
				return true;
			}
		}

		/// <summary>
		/// Adds a new function to the queue
		/// </summary>
		/// <typeparam name="TState"></typeparam>
		/// <typeparam name="TReturn"></typeparam>
		/// <param name="state"></param>
		/// <param name="function"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public Task<TReturn> EnqueueAsync<TState, TReturn>(TState state, Func<TState, Task<TReturn>> function)
		{
			if(function is null) throw new ArgumentNullException(nameof(function));

			var stateData = new FuncStateData_Return<TState, TReturn>(state, function);

			lock(m_SyncRoot)
			{
				Schedule(stateData);
				return stateData.Task;
			}
		}

		/// <summary>
		/// Attempts to enqueue a function
		/// </summary>
		/// <typeparam name="TState"></typeparam>
		/// <typeparam name="TReturn"></typeparam>
		/// <param name="state"></param>
		/// <param name="function"></param>
		/// <param name="task"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public bool TryEnqueueAsync<TState, TReturn>(TState state, Func<TState, Task<TReturn>> function, [NotNullWhen(true)] out Task<TReturn>? task)
		{
			if(function is null) throw new ArgumentNullException(nameof(function));

			lock(m_SyncRoot)
			{
				if(m_Disposed)
				{
					task = null;
					return false;
				}

				var stateData = new FuncStateData_Return<TState, TReturn>(state, function);
				Schedule(stateData);
				
				task = stateData.Task;
				return true;
			}
		}

		/// <summary>
		/// Adds a new function to the queue
		/// </summary>
		/// <typeparam name="TReturn"></typeparam>
		/// <param name="function"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public Task<TReturn> EnqueueAsync<TReturn>(Func<Task<TReturn>> function)
		{
			if(function is null) throw new ArgumentNullException(nameof(function));

			var stateData = new FuncStateData_Return<TReturn>(function);

			lock(m_SyncRoot)
			{
				Schedule(stateData);
				return stateData.Task;
			}
		}

		public bool TryEnqueueAsync<TReturn>(Func<Task<TReturn>> function, [NotNullWhen(true)] out Task<TReturn>? task)
		{
			if(function is null) throw new ArgumentNullException(nameof(function));			

			lock(m_SyncRoot)
			{
				if(m_Disposed)
				{
					task = null;
					return false;
				}

				var stateData = new FuncStateData_Return<TReturn>(function);
				Schedule(stateData);
				
				task = stateData.Task;
				return true;
			}
		}

		/// <summary>
		/// Adds a function to the queue
		/// </summary>
		/// <typeparam name="TState"></typeparam>
		/// <param name="state"></param>
		/// <param name="function"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public Task EnqueueAsync<TState>(TState state, Func<TState, Task> function)
		{	
			if(function is null) throw new ArgumentNullException(nameof(function));

			var stateData = new FuncStateData<TState>(state, function);

			lock(m_SyncRoot)
			{
				Schedule(stateData);
				return stateData.Task;
			}
		}

		/// <summary>
		/// Attempts to add a new function to the queue
		/// </summary>
		/// <typeparam name="TState"></typeparam>
		/// <param name="state"></param>
		/// <param name="function"></param>
		/// <param name="task"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public bool TryEnqueueAsync<TState>(TState state, Func<TState, Task> function, [NotNullWhen(true)] out Task? task)
		{	
			if(function is null) throw new ArgumentNullException(nameof(function));

			lock(m_SyncRoot)
			{
				if(m_Disposed)
				{
					task = null;
					return false;
				}

				var stateData = new FuncStateData<TState>(state, function);
				Schedule(stateData);
				
				task = stateData.Task;
				return true;
			}
		}

		private void ContextEnqueue(SendOrPostCallback callback, object? state)
		{
			var stateData = new SendOrPostStateData(callback, state);

			lock(m_SyncRoot)
			{
				Schedule(stateData);
			}
			
		}

		private void Schedule(StateData stateData)
		{
			if(m_Disposed) throw new ObjectDisposedException(nameof(AsyncWorkQueue));

			m_Data.Add(stateData);

			if(m_ThreadActive == false)
			{
				m_ThreadActive = true;
				m_Dispatcher.QueueUserWorkItem(m_ProcessQueue);
			}
		}

		private void ProcessQueue(object? state)
		{
			lock(m_SyncRoot)
			{
				var previousContext = SynchronizationContext.Current;

				try
				{
					s_ActiveID.Value = m_ID;
					SynchronizationContext.SetSynchronizationContext(m_QueueContext);

					while(m_Data.Count != 0 && m_StopProcessing == false)
					{
						var temp = m_Data;
						m_Data = m_SwapData;
						m_SwapData = temp;

						Monitor.Exit(m_SyncRoot);

						try
						{
							ProcessItems(m_SwapData);
						}
						finally
						{
							// We need to clear out the data
							m_SwapData.Clear();

							// And reaquire the lock here
							Monitor.Enter(m_SyncRoot);
						}
					}
				}
				finally
				{
					m_ThreadActive = false;

					if(m_StopProcessing)
					{
						ProcessItems(m_Data);
						m_StopEvent.Set();
					}

					SynchronizationContext.SetSynchronizationContext(previousContext);
					s_ActiveID.Value = NoActiveQueue;
				}
			}
		}

		private void ProcessItems(List<StateData> items)
		{
			for(int i = 0; i < items.Count; i++)
			{
				var item = items[i];
				item.Execute();
			}
		}

		private bool RaiseUnhandledThreadException(Exception exception)
		{
			var handler = UnhandledExceptionOnSynchronizationContext;
			if(handler is null) return false;

			var args = new UnhandledAsyncWorkQueueExceptionEventArgs(exception);
			handler(this, args);

			return args.Rethrow;
		}
		
		abstract class StateData
		{
			public abstract void Execute();
		}

		sealed class SendOrPostStateData : StateData
		{
			private readonly SendOrPostCallback m_Callback;
			private readonly object? m_State;

			public SendOrPostStateData(SendOrPostCallback callback, object? state)
			{
				m_Callback = callback;
				m_State = state;
			}

			public override void Execute()
			{
				m_Callback(m_State);
			}
		}

		abstract class TaskStateDataBase<T> : StateData
		{
			protected readonly TaskCompletionSource<T> m_Tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

			public Task<T> Task
			{
				get{return m_Tcs.Task;}
			}
		}

		sealed class ActionStateData : TaskStateDataBase<bool>
		{
			private Action m_Action;

			public ActionStateData(Action action)
			{
				m_Action = action;
			}

			public override void Execute()
			{
				try
				{
					m_Action();
					m_Tcs.SetResult(true);
				}
				catch(Exception e)
				{
					m_Tcs.SetException(e);
				}
			}
		}

		sealed class ActionStateData<TState> : TaskStateDataBase<bool>
		{
			private Action<TState> m_Action;
			private readonly TState m_State;

			public ActionStateData(TState state, Action<TState> action)
			{
				m_State = state;
				m_Action = action;
			}

			public override void Execute()
			{
				try
				{
					m_Action(m_State);
					m_Tcs.SetResult(true);
				}
				catch(Exception e)
				{
					m_Tcs.SetException(e);
				}
			}
		}

		sealed class FuncStateData : TaskStateDataBase<bool>
		{
			private readonly Func<Task> m_Function;
				
			public FuncStateData(Func<Task> function)
			{
				m_Function = function;
			}

			public override async void Execute()
			{
				try
				{
					await m_Function();
					m_Tcs.SetResult(true);
				}
				catch(Exception e)
				{
					m_Tcs.SetException(e);
				}
			}
		}

		sealed class FuncStateData<TState> : TaskStateDataBase<bool>
		{
			private readonly Func<TState, Task> m_Function;
			private readonly TState m_State;
				
			public FuncStateData(TState state, Func<TState, Task> function)
			{
				m_State = state;
				m_Function = function;
			}

			public override async void Execute()
			{
				try
				{
					await m_Function(m_State);
					m_Tcs.SetResult(true);
				}
				catch(Exception e)
				{
					m_Tcs.SetException(e);
				}
			}
		}

		sealed class FuncStateData_Return<TReturn> : TaskStateDataBase<TReturn>
		{
			private readonly Func<Task<TReturn>> m_Function;
				
			public FuncStateData_Return(Func<Task<TReturn>> function)
			{
				m_Function = function;
			}

			public override async void Execute()
			{
				try
				{
					var result = await m_Function();
					m_Tcs.SetResult(result);
				}
				catch(Exception e)
				{
					m_Tcs.SetException(e);
				}
			}
		}

		sealed class FuncStateData_Return<TState, TReturn> : TaskStateDataBase<TReturn>
		{
			private readonly Func<TState, Task<TReturn>> m_Function;
			private readonly TState m_State;
				
			public FuncStateData_Return(TState state, Func<TState, Task<TReturn>> function)
			{
				m_State = state;
				m_Function = function;
			}

			public override async void Execute()
			{
				try
				{
					var result = await m_Function(m_State);
					m_Tcs.SetResult(result);
				}
				catch(Exception e)
				{
					m_Tcs.SetException(e);
				}
			}
		}

		sealed class NoTaskFunctionStateData<TState> : StateData
		{
			private readonly Func<TState, Task> m_Function;
			private readonly TState m_State;
				
			public NoTaskFunctionStateData(TState state, Func<TState, Task> function)
			{
				m_State = state;
				m_Function = function;
			}

			public override async void Execute()
			{
				try
				{
					await m_Function(m_State);
				}
				catch(Exception)
				{
				}
			}
		}
	}
}
