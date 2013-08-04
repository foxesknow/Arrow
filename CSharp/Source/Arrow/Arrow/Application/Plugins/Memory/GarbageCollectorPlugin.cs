using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Arrow.Application.Plugins.Memory
{
	/// <summary>
	/// Periodically collections garbage within the application
	/// </summary>
	public class GarbageCollectorPlugin : IPausablePlugin, IDisposable
	{
		private Timer m_Timer;
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		public GarbageCollectorPlugin()
		{
			this.Interval=new TimeSpan(0,1,0); // One minute
		}

		/// <summary>
		/// How often to garbage collect. The default is every minute
		/// </summary>
		public TimeSpan Interval{get;set;}
		
		/// <summary>
		/// How often to garbage collect. The default is every minute
		/// </summary>
		private void TimerElapsed(object sender, ElapsedEventArgs e)
		{
			GC.Collect();
		}
	
		/// <summary>
		/// Pauses garbage collection
		/// </summary>
		public void Pause()
		{
			if(m_Timer!=null) m_Timer.Enabled=false;
		}

		/// <summary>
		/// Continues garbage collection
		/// </summary>
		public void Continue()
		{
			if(m_Timer!=null) m_Timer.Enabled=true;
		}

		/// <summary>
		/// Starts garbage collection
		/// </summary>
		public void Start()
		{
			if(m_Timer==null)
			{
				m_Timer=new Timer();
				m_Timer.Interval=this.Interval.TotalMilliseconds;
				m_Timer.Elapsed+=TimerElapsed;
			}
		}

		/// <summary>
		/// Stops garbage collection
		/// </summary>
		public void Stop()
		{
			if(m_Timer!=null)
			{
				m_Timer.Stop();
				m_Timer.Dispose();
				m_Timer=null;
			}
		}

		/// <summary>
		/// The name of the service
		/// </summary>
		public string Name
		{
			get{return "GarbageCollector";}
		}

		/// <summary>
		/// Stops collection and disposes of any resources
		/// </summary>
		public void Dispose()
		{
			Stop();
		}

	}
}
