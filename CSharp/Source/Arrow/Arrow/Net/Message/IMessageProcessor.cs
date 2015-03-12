using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Net.Message
{
	/// <summary>
	/// Defines the common behaviour for all message processors
	/// </summary>
	public interface IMessageProcessor
	{		
		/// <summary>
		/// Called when the socket processor detects a disconnect from the client
		/// </summary>
		/// <param name="socketProcessor"></param>
		void HandleDisconnect(SocketProcessor socketProcessor);

		/// <summary>
		/// Called when the socket processor detects a network fault
		/// </summary>
		/// <param name="socketProcessor"></param>
		void HandleNetworkFault(SocketProcessor socketProcessor);
	}

	/// <summary>
	/// Defines the behaviour for a header/body processor
	/// </summary>
	/// <typeparam name="THeader">The type of the header</typeparam>
	/// <typeparam name="TBody">The type of the body</typeparam>
	public interface IMessageProcessor<in THeader,in TBody> : IMessageProcessor
	{			
		/// <summary>
		/// Processes a message
		/// </summary>
		/// <param name="socketProcessor">The socket processor that read the message</param>
		/// <param name="header">The message header</param>
		/// <param name="body">The message body</param>
		/// <returns>What the processor should do next</returns>
		ReadMode Process(SocketProcessor socketProcessor, THeader header, TBody body);
	}
}
