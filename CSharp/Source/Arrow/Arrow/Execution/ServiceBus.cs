using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Execution
{
	/// <summary>
	/// Provides a central place to register for events.
	/// This class is thread-safe
	/// </summary>
	public class ServiceBus
	{
		private const int BucketMask=31;
               
		private readonly object m_Lock=new object();
		private readonly EventList[] m_Handlers=new EventList[BucketMask+1];

		/// <summary>
		/// Initializes the instance
		/// </summary>
		public ServiceBus()
		{
			for(int i=0; i<m_Handlers.Length; i++)
			{
				m_Handlers[i]=new EventList();
			}
		}

		/// <summary>
		/// Registers a new handler
		/// </summary>
		/// <typeparam name="T">The type of event to handle</typeparam>
		/// <param name="handler">The handler for the event</param>
		public void Register<T>(EventHandler<T> handler) where T:EventArgs
		{
			if(handler==null) throw new ArgumentNullException("handler");
	       
			IntPtr handle=typeof(T).TypeHandle.Value;
			int bucket=(handle.GetHashCode()>>3)&BucketMask;
			
			List<EventInfo> handlers=new List<EventInfo>();		       
			EventInfo info=new EventInfo(handler,handle);
	       
			lock(m_Lock)
			{
				handlers.AddRange(m_Handlers[bucket].Events);
				handlers.Add(info);
				
				m_Handlers[bucket].Events=handlers.ToArray();
			}
		}

		/// <summary>
		/// Removes an event handler
		/// </summary>
		/// <typeparam name="T">The type of event the handler handles</typeparam>
		/// <param name="handler">The handler to remove</param>
		public void Remove<T>(EventHandler<T> handler) where T:EventArgs
		{
			if(handler==null) throw new ArgumentNullException("handler");
			
			IntPtr handle=typeof(T).TypeHandle.Value;
			int bucket=(handle.GetHashCode()>>3)&BucketMask;
	       
			List<EventInfo> handlers=new List<EventInfo>();
	       
			lock(m_Lock)
			{
				handlers.AddRange(m_Handlers[bucket].Events);
				handlers.RemoveAll(e=>
				{
					var other=e.Handler as EventHandler<T>;
					return other!=null && other==handler;
				});
				
				m_Handlers[bucket].Events=handlers.ToArray();
			}
		}

		/// <summary>
		/// Raises an event against each handler that takes a T
		/// </summary>
		/// <typeparam name="T">The type of the event</typeparam>
		/// <param name="sender">The sender of the event (may be null)</param>
		/// <param name="args">The event arguments (may be null)</param>
		public void Raise<T>(object sender, T args) where T:EventArgs
		{
			IntPtr handle=typeof(T).TypeHandle.Value;
			int bucket=(handle.GetHashCode()>>3)&BucketMask;
			
			var handlers=m_Handlers[bucket].Events;
			if(handlers.Length==0) return;
	       
			for(int i=0; i<handlers.Length; i++)
			{
				EventInfo info=handlers[i];
				if(info.Handle==handle)
				{
					EventHandler<T> handler=(EventHandler<T>)info.Handler;
					handler(sender,args);
				}
			}
		}
		
		/// <summary>
		/// Raises an event against each handler that takes a T.
		/// The 'raiser' will be passed an action that can be called to raise the event
		/// </summary>
		/// <typeparam name="T">The type of the event</typeparam>
		/// <param name="sender">The sender of the event (may be null)</param>
		/// <param name="args">The event arguments (may be null)</param>
		/// <param name="raiser">A delegate that is responsible for executing the method</param>
		public void RaiseVia<T>(object sender, T args, Action<Action> raiser) where T:EventArgs
		{
			if(raiser==null) throw new ArgumentNullException("raiser");
		
			IntPtr handle=typeof(T).TypeHandle.Value;
			int bucket=(handle.GetHashCode()>>3)&BucketMask;
			
			var handlers=m_Handlers[bucket].Events;
			if(handlers.Length==0) return;
	       
			for(int i=0; i<handlers.Length; i++)
			{
				EventInfo info=handlers[i];
				if(info.Handle==handle)
				{
					EventHandler<T> handler=(EventHandler<T>)info.Handler;
					Action action=()=>handler(sender,args);
					raiser(action);
				}
			}
		}

		class EventList
		{
			public volatile EventInfo[] Events=new EventInfo[0];
		}

		struct EventInfo
		{
			public readonly object Handler;
			public readonly IntPtr Handle;
	       
			public EventInfo(object handler, IntPtr handle)
			{
				Handler=handler;
				Handle=handle;
			}
		} 
	}
}
