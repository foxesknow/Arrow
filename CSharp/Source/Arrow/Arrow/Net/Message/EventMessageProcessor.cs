using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Net.Message
{
	/// <summary>
	/// A message processor implementation that raises events when things happen
	/// </summary>
	/// <typeparam name="THeader">The message header</typeparam>
	/// <typeparam name="TBody">The message body</typeparam>
	public class EventMessageProcessor<THeader,TBody> : IMessageProcessor<THeader,TBody>
	{
		/// <summary>
		/// Raised when a new message is received
		/// </summary>
		public EventHandler<SocketMessageEventArgs<THeader,TBody>> Message;
		
		/// <summary>
		/// Raised when a disconnect is detected
		/// </summary>
		public EventHandler<SocketProcessorEventArgs> Disconnect;
		
		/// <summary>
		/// Raised when a network fault is detected
		/// </summary>
		public EventHandler<SocketProcessorEventArgs> NetworkFault;

		ReadMode IMessageProcessor<THeader,TBody>.ProcessMessage(SocketProcessor socketProcessor, THeader header, TBody body)
		{
			ReadMode readMode=ReadMode.KeepReading;

			var d=Message;
			if(d!=null)
			{
				var args=new SocketMessageEventArgs<THeader,TBody>(socketProcessor,header,body);
				d(this,args);

				readMode=args.ReadMode;
			}

			return readMode;
		}

		void IMessageProcessor.HandleDisconnect(SocketProcessor socketProcessor)
		{
			var d=Disconnect;
			if(d!=null)
			{
				d(this,new SocketProcessorEventArgs(socketProcessor));
			}
		}

		void IMessageProcessor.HandleNetworkFault(SocketProcessor socketProcessor)
		{
			var d=NetworkFault;
			if(d!=null)
			{
				d(this,new SocketProcessorEventArgs(socketProcessor));
			}
		}
	}
}
