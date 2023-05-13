using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Arrow.Execution;
using Arrow.Collections;
using Arrow.Threading;
using Arrow.Threading.Collections;
using Arrow.Serialization;

namespace Arrow.Messaging.Memory
{
	/// <summary>
	/// Base class for all sessions
	/// </summary>
	abstract class Session : IDisposable
	{
		private readonly object m_SyncRoot=new object();
		private readonly SwapList<EventHandler<MessageEventArgs>> m_MessagesAvailableHandlers=new SwapList<EventHandler<MessageEventArgs>>();

		private long m_Usage;

		private readonly Queue<byte[]> m_Queue=new Queue<byte[]>();
		private readonly Uri m_Address;

		private bool m_ThreadActive;

		private readonly IWorkDispatcher m_Dispatcher=new ThreadPoolWorkDispatcher();

		protected Session(Uri address)
		{
			m_Address=address;
		}

		/// <summary>
		/// Raised then a message is available on the session
		/// </summary>
		public event EventHandler<MessageEventArgs> MessageAvailable
		{
			add
			{
                if(value == null) return;

                lock(m_SyncRoot)
                {
                    m_MessagesAvailableHandlers.Add(value);
                    ScheduleProcessing();
                }
            }

			remove
			{
                if(value == null) return;

                lock(m_SyncRoot)
                {
                    m_MessagesAvailableHandlers.Remove(value);
                }
            }
		}

		/// <summary>
		/// Returns the number of MessageAvailable event handlers
		/// </summary>
		protected int MessageAvailableCount
		{
			get
			{
				lock(m_SyncRoot)
				{
					return m_MessagesAvailableHandlers.Count;
				}
			}
		}

		protected void OnMessageAvailable(Func<MessageEventArgs> argFactory)
		{
			foreach(var d in m_MessagesAvailableHandlers)
			{
				MethodCall.AllowFail(()=>
				{
					var args = argFactory();
					d(this, args);
				});
			}
		}

        /// <summary>
        /// Sends a message
        /// </summary>
        /// <param name="message"></param>
        public virtual void Send(IMessage message)
        {
            byte[] data = GenericBinaryFormatter.ToArray(message);

            lock(m_SyncRoot)
            {
                m_Queue.Enqueue(data);
                ScheduleProcessing();
            }
        }

        private void ScheduleProcessing()
        {
            if(m_ThreadActive == false)
            {
                m_ThreadActive = true;
                m_Dispatcher.QueueUserWorkItem(ProcessQueue);
            }
        }

        /// <summary>
        /// Closes the session
        /// </summary>
        public void Close()
        {
            Release();
        }

        /// <summary>
        /// Attempts to get a message from the queue
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool TryGetMessage(out IMessage message)
        {
            bool gotMessage = false;
            byte[] messageBytes = null;

            message = null;

            lock(m_SyncRoot)
            {
                if(m_Queue.Count != 0)
                {
                    gotMessage = true;
                    messageBytes = m_Queue.Dequeue();
                }
            }

            if(gotMessage) message = GenericBinaryFormatter.FromArray<IMessage>(messageBytes);

            return gotMessage;
        }

        protected bool TryGetBytes(out byte[] messageData)
        {
            lock(m_SyncRoot)
            {
                if(m_Queue.Count != 0)
                {
                    messageData = m_Queue.Dequeue();
                    return true;
                }
                else
                {
                    messageData = null;
                    return false;
                }
            }
        }

        /// <summary>
        /// The address of the topic or queue
        /// </summary>
        public Uri Address
		{
			get{return m_Address;}
		}

		public void Dispose()
		{
			Close();
		}

		/// <summary>
		/// Adds a reference to the queue
		/// </summary>
		public void AddRef()
		{
			lock(m_SyncRoot)
			{
				m_Usage++;
			}
		}

        /// <summary>
        /// Release a reference to the queue.
        /// When there are no more references any resources will be reclaimed
        /// </summary>
        public void Release()
        {
            lock(m_SyncRoot)
            {
                if(m_Usage > 0)
                {
                    m_Usage--;
                    if(m_Usage == 0)
                    {
                        // Just for sanity, when there's nobody using the
                        // session make sure we've removed all events
                        m_MessagesAvailableHandlers.Clear();
                    }
                }
            }
        }

        /// <summary>
        /// Indicates if messages should be buffers (for a queue)
        /// or thrown away if nobody is subscribed (for a topic)
        /// </summary>
        protected bool BufferMessages{get; set;}

        private void ProcessQueue(object state)
        {
            lock(m_SyncRoot)
            {
                try
                {
                    while(true)
                    {
                        // If theres nothing to process then stop
                        if(m_Queue.Count == 0) return;

                        // If we're buffering messages (ie a queue) and nobody is listening then stop
                        if(this.BufferMessages && m_MessagesAvailableHandlers.Count == 0) return;

                        // We need to exit the lock so that other threads
                        // can modify the topic/queue whilst in their callbacks
                        Monitor.Exit(m_SyncRoot);

                        try
                        {
                            DispatchMessage();
                        }
                        finally
                        {
                            Monitor.Enter(m_SyncRoot);
                        }
                    }
                }
                finally
                {
                    m_ThreadActive = false;
                }
            }
        }

        protected abstract void DispatchMessage();

        public override string ToString()
        {
            return m_Address.ToString();
        }
    }
}
