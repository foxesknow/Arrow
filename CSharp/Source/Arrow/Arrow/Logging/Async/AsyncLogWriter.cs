using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Arrow.Logging.Async
{
	/// <summary>
	/// Provides a centralized means to write to log files asynchronously.
	/// </summary>
	public static class AsyncLogWriter
	{
		private static readonly object s_Lock=new object();
		private static Thread? s_LogThread;
		private static bool s_Running;
		
		private static List<ILogData> s_LogData=new List<ILogData>();
		private static List<ILogData> s_LogDataSwap=new List<ILogData>();
		
		static AsyncLogWriter()
		{
			LogManager.StopLogging+=(s,e)=>StopLogThread();
			StartLogThread();
		}
		
		/// <summary>
		/// Enqueues log data for processing on the log writer thread
		/// </summary>
		/// <param name="data">The data to write to a log</param>
		public static void Enqueue(ILogData data)
		{
			lock(s_Lock)
			{
				if(s_Running)
				{
					s_LogData.Add(data);
					Monitor.Pulse(s_Lock);
				}
			}
		}
		
		/// <summary>
		/// Starts the log thread.
		/// There will only be one instance of this thread
		/// </summary>
		private static void StartLogThread()
		{
			s_Running=true;
		
			s_LogThread=new Thread(LoggingMain);
			s_LogThread.Name="AsyncLogWriter";
			s_LogThread.IsBackground=true;
			
			s_LogThread.Start();
		}
		
		/// <summary>
		/// Stops the logging thread and give it time to flush any pending items
		/// </summary>
		private static void StopLogThread()
		{
			if(s_LogThread!=null)
			{
				lock(s_Lock)
				{
					s_Running=false;
					Monitor.Pulse(s_Lock);
				}
			
				// We'll give it some time to stop, but ultimately if it 
				// doesn't then we can carry on as it's a background thread
				s_LogThread.Join(20*1000);
				s_LogThread=null;
				s_Running=false;				
			}
		}
		
		/// <summary>
		/// Processes any log items than have been enqueued
		/// </summary>
		private static void LoggingMain()
		{
			lock(s_Lock)
			{
				while(s_Running)
				{
					while(s_LogData.Count==0)
					{
						Monitor.Wait(s_Lock);
						if(s_Running==false) break;
					}
					
					if(s_Running==false) break;
					
					// Swap the queues for performance
					var data=s_LogData;
					s_LogData=s_LogDataSwap;
					s_LogDataSwap=data;
					
					Monitor.Exit(s_Lock);
					try
					{
						Process(data);
					}
					finally
					{
						data.Clear();
						Monitor.Enter(s_Lock);
					}
				}
			}
			
			// When we get here the thread is about to exit due to a shutdown.
			// Do the final updates here so that everything happens on the thread
			Process(s_LogData);
			s_LogData.Clear();
		}
		
		/// <summary>
		/// Processes a list of LogData 
		/// </summary>
		/// <param name="data">The items to log</param>
		private static void Process(List<ILogData> data)
		{
			for(int i=0; i<data.Count; i++)
			{
				try
				{
					ILogData d=data[i];
					d.WriteToLog();
				}
				catch
				{
					// Ignore it
				}
			}
		}
	}
}
