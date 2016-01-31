using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Arrow.Church.Common;
using Arrow.Church.Common.Data;
using Arrow.Church.Common.Internal;
using Arrow.Church.Common.Net;
using Arrow.Memory;
using Arrow.Net.Message;
using Arrow.Threading;

namespace Arrow.Church.Client.ServiceDispatchers
{
	public class NetworkServiceDispatcher : ServiceDispatcher
	{
		private readonly IPAddress m_Address;

		private SocketProcessor m_SocketProcessor;

		private readonly EventMessageProcessor<MessageEnvelope,byte[]> m_MessageProcessor=new EventMessageProcessor<MessageEnvelope,byte[]>();
		private readonly IMessageFactory<MessageEnvelope,byte[]> m_MessageFactory=new MessageEnvelopeMessageFactory();


		public NetworkServiceDispatcher(Uri endpoint) : base(endpoint)
		{
			m_Address=endpoint.TryResolveIPAddress();
			if(m_Address==null) throw new ArgumentException("could not resolve host: "+endpoint.Host.ToString(),"endpoint");

			var socket=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
			socket.Connect(m_Address,this.Endpoint.Port);

			m_MessageProcessor.Message+=HandleMessage;
			m_MessageProcessor.Disconnect+=HandleDisconnect;
			m_MessageProcessor.NetworkFault+=HandleNetworkFault;

			m_SocketProcessor=new FixedHeaderSocketProcessor<MessageEnvelope,byte[]>(socket,m_MessageFactory,m_MessageProcessor);
			m_SocketProcessor.Start();
		}

		private void HandleMessage(object sender, SocketMessageEventArgs<MessageEnvelope,byte[]> args)
		{
			HandleResponse(args.Header,ArraySegmentCollection.FromArray(args.Body));
			args.ReadMode=ReadMode.KeepReading;
		}

		private void HandleDisconnect(object sender, SocketProcessorEventArgs args)
		{
			CompleteAllError(new ChurchException("disconnect for "+this.Endpoint));
		}

		private void HandleNetworkFault(object sender, SocketProcessorEventArgs args)
		{
			CompleteAllError(new ChurchException("network fault for "+this.Endpoint));
		}

		protected override Task SendRequestAsync(MessageEnvelope envelope, byte[] data)
		{
			try
			{
				using(var stream=new MemoryStream())
				using(var encoder=new DataEncoder(stream))
				{
					encoder.WriteNeverNull(envelope);

					var segments=ArraySegmentCollection.FromMemoryStream(stream);
					segments.AddBack(new ArraySegment<byte>(data));

					return m_SocketProcessor.WriteAsync(segments.UnderlyingSegments);
				}
			}
			catch(Exception e)
			{
				return TaskEx.FromException(e);
			}
		}

		public override void Dispose()
		{
			m_SocketProcessor.Dispose();
			base.Dispose();
		}
	}
}
