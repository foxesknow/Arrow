using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Application.Plugins.Memory
{
	/// <summary>
	/// Places additional stress on the garbage collector.
	/// This is useful for stress testing an application as it will cause more frequent collections
	/// </summary>
	public sealed class MemoryPressurePlugin : Plugin
	{
		private bool m_Started;
	
		/// <summary>
		/// The number of bytes to allocate
		/// </summary>
		public long Bytes{get; set;}
	
		/// <summary>
		/// Starts the plugin
		/// </summary>
		protected internal override ValueTask Start()
        {
            if(m_Started == false)
            {
                GC.AddMemoryPressure(this.Bytes);
                m_Started = true;
            }

			return default;
        }

        /// <summary>
        /// Stops the plugin
        /// </summary>
        protected internal override ValueTask Stop()
		{
            if(m_Started)
            {
                GC.RemoveMemoryPressure(this.Bytes);
                m_Started = false;
            }

			return default;
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
