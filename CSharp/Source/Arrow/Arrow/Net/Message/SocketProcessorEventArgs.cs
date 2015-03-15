using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Net.Message
{
	/// <summary>
	/// Base class for socket processor events raises by the EventMessageProcessor
	/// </summary>
	public class SocketProcessorEventArgs : EventArgs
	{
		private readonly SocketProcessor m_SocketProcessor;

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="socketProcessor">The socket processor that is running</param>
		public SocketProcessorEventArgs(SocketProcessor socketProcessor)
		{
			if(socketProcessor==null) throw new ArgumentNullException("socketProcessor");

			m_SocketProcessor=socketProcessor;
		}

		/// <summary>
		/// The socket processor
		/// </summary>
		public SocketProcessor SocketProcessor
		{
			get{return m_SocketProcessor;}
		}
	}
}
