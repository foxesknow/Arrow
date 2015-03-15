using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Net.Message
{
	/// <summary>
	/// Holds the outcome as an async write to a socket processor
	/// </summary>
	public struct WriteResults
	{
		private readonly SocketProcessor m_SocketProcessor;
		private readonly int m_BytesWritten;

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="socketProcessor">The socket processor that was written to</param>
		/// <param name="bytesWritten">The number of bytes that were written</param>
		public WriteResults(SocketProcessor socketProcessor, int bytesWritten)
		{
			m_SocketProcessor=socketProcessor;
			m_BytesWritten=bytesWritten;
		}

		/// <summary>
		/// The socket processor that was written to
		/// </summary>
		public SocketProcessor SocketProcessor
		{
			get{return m_SocketProcessor;}
		}

		/// <summary>
		/// The number of bytes written
		/// </summary>
		public int BytesWritten
		{
			get{return m_BytesWritten;}
		}
	}
}
