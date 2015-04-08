using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Application.Plugins.Memory
{
	/// <summary>
	/// Places additional stress on the garbage collector.
	/// This is useful for stress testing an application as it will cause more frequent collections
	/// </summary>
	public class MemoryPressurePlugin : Plugin
	{
		private volatile bool m_Started;
	
		/// <summary>
		/// The number of bytes to allocate
		/// </summary>
		public long Bytes{get;set;}
	
		/// <summary>
		/// Starts the plugin
		/// </summary>
		protected internal override void Start()
		{
			if(m_Started==false)
			{
				GC.AddMemoryPressure(this.Bytes);
				m_Started=true;
			}
		}

		/// <summary>
		/// Stops the plugin
		/// </summary>
		protected internal override void Stop()
		{
			if(m_Started)
			{
				GC.RemoveMemoryPressure(this.Bytes);
				m_Started=false;
			}
		}

		/// <summary>
		/// The name of the plugin
		/// </summary>
		public override string Name
		{
			get{return "MemoryPressure";}
		}

	}
}
