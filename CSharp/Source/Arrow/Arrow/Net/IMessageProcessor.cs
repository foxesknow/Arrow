using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Net
{
	public interface IMessageProcessor<in THeader,in TBody>
	{
		/// <summary>
		/// Processes a message
		/// </summary>
		/// <param name="socketProcessor">The socket processor that read the message</param>
		/// <param name="header">The message header</param>
		/// <param name="body">The message body</param>
		/// <returns>What the processor should do next</returns>
		ReadMode Process(SocketProcessor socketProcessor, THeader header, TBody body);

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
}
