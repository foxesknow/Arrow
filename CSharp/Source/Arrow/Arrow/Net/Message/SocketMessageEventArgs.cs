using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Net.Message
{
	/// <summary>
	/// Event args for a new message
	/// </summary>
	/// <typeparam name="THeader">The message header</typeparam>
	/// <typeparam name="TBody">The message body</typeparam>
	public class SocketMessageEventArgs<THeader,TBody> : EventArgs
	{
		private readonly SocketProcessor m_SocketProcessor;
		private readonly THeader m_Header;
		private readonly TBody m_Body;

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="socketProcessor">The socket processor that read the message</param>
		/// <param name="header">The message header</param>
		/// <param name="body">The message body</param>
		public SocketMessageEventArgs(SocketProcessor socketProcessor, THeader header, TBody body)
		{
			if(socketProcessor==null) throw new ArgumentNullException("socketProcessor");
			if(header==null) throw new ArgumentNullException("header");
			if(body==null) throw new ArgumentNullException("body");

			m_SocketProcessor=socketProcessor;
			m_Header=header;
			m_Body=body;
			this.ReadMode=Message.ReadMode.KeepReading;
		}

		/// <summary>
		/// The socket processor
		/// </summary>
		public SocketProcessor SocketProcessor
		{
			get{return m_SocketProcessor;}
		}

		/// <summary>
		/// The message header
		/// </summary>
		public THeader Header
		{
			get{return m_Header;}
		}

		/// <summary>
		/// The message body
		/// </summary>
		public TBody Body
		{
			get{return m_Body;}
		}

		/// <summary>
		/// What the processor should do next.
		/// The default is to keep reading more messages
		/// </summary>
		public ReadMode ReadMode{get;set;}
	}
}
