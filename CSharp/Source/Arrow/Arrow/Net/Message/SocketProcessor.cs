using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Arrow.Net.Message
{
	/// <summary>
	/// Base class for all socket processors
	/// </summary>
	public abstract class SocketProcessor
	{
		private readonly IMessageProcessor m_MessageProcessor;

		private long m_Closed;

		protected SocketProcessor(IMessageProcessor messageProcessor)
		{
			if(messageProcessor==null) throw new ArgumentNullException("messageProcessor");

			m_MessageProcessor=messageProcessor;
		}

		/// <summary>
		/// Determines if the processor should deal with the bytes read from a socket.
		/// This is typically called on an EndRead to determine if the socket was connected
		/// </summary>
		/// <param name="bytesRead">The number of bytes read</param>
		/// <returns>true if the caller should handle the bytes, otherwise false</returns>
		protected bool CanProcessBytesRead(int bytesRead)
		{
			if(bytesRead==0)
			{
				OnDisconnected();
				return false;
			}
			else
			{
				return true;
			}
		}

		/// <summary>
		/// Flags the network system as being closed.
		/// This allows the processor to work out how to handle exceptions thrown by network calls
		/// </summary>
		protected void FlagAsClosed()
		{
			Interlocked.Exchange(ref m_Closed,1);
		}

		/// <summary>
		/// Executes a piece of network code and deals with any errors
		/// </summary>
		/// <param name="action">The network code to execute</param>
		/// <returns>true if the network code executes successfully, otherwise false</returns>
		protected bool HandleNetworkCall(Action action)
		{
			try
			{
				action();
				return true;
			}
			catch
			{
				if(Interlocked.Read(ref m_Closed)!=1)
				{
					m_MessageProcessor.HandleNetworkFault(this);
				}
			}

			return false;
		}

		/// <summary>
		/// Starts reading messages from the socket
		/// </summary>
		public abstract void Start();

		/// <summary>
		/// Closes the socket processor
		/// </summary>
		public abstract void Close();

		/// <summary>
		/// Called when a network disconnect is detected
		/// </summary>
		protected abstract void OnDisconnected();


		/// <summary>
		/// Writes data to the socket
		/// </summary>
		/// <param name="buffer">The data to write</param>
		/// <param name="offset">The start of the data within the buffer</param>
		/// <param name="size">How much data to write</param>
		/// <returns>A task that will be signalled when the write completes</returns>
		public abstract Task<SocketProcessor> WriteAsync(byte[] buffer, int offset, int size);

		/// <summary>
		/// Writes data to the socket
		/// </summary>
		/// <param name="buffer">The data to write</param>
		/// <param name="offset">The start of the data within the buffer</param>
		/// <param name="size">How much data to write</param>
		public abstract void Write(byte[] buffer, int offset, int size);

	}
}
