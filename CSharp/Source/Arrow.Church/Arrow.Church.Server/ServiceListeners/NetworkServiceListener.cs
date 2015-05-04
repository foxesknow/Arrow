using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using Arrow.Church.Common.Net;
using Arrow.Net.Message;
using Arrow.Threading;

namespace Arrow.Church.Server.ServiceListeners
{
	public partial class NetworkServiceListener : ServiceListener
	{
		private readonly object m_SyncRoot=new object();
		private readonly Dictionary<long,SocketProcessor> m_Processors=new Dictionary<long,SocketProcessor>();

		private readonly IWorkDispatcher m_CallDispatcher=new ThreadPoolWorkDispatcher();

		private readonly Uri m_Endpoint;
		private readonly IPAddress m_Address;
		private readonly TcpListener m_TcpListener;
		
		private readonly EventMessageProcessor<MessageEnvelope,byte[]> m_MessageProcessor=new EventMessageProcessor<MessageEnvelope,byte[]>();
		private readonly IMessageFactory<MessageEnvelope,byte[]> m_MessageFactory=new NetworkServiceMessageFactory();

		public NetworkServiceListener(Uri endpoint)
		{
			if(endpoint==null) throw new ArgumentNullException("endpoint");
			m_Endpoint=endpoint;

			m_Address=ResolveIPAddress(endpoint.Host);
			if(m_Address==null) throw new ArgumentException("could not resolve host: "+endpoint.Host.ToString(),"endpoint");

			m_MessageProcessor.Message+=HandleMessage;
			m_MessageProcessor.Disconnect+=HandleDisconnect;
			m_MessageProcessor.NetworkFault+=HandleNetworkFault;

			m_TcpListener=new TcpListener(m_Address,endpoint.Port);
			m_TcpListener.BeginAcceptSocket(HandleAcceptSocket,null);
		}

		private void HandleAcceptSocket(IAsyncResult result)
		{
			var socket=m_TcpListener.EndAcceptSocket(result);

			var processor=new FixedHeaderSocketProcessor<MessageEnvelope,byte[]>(socket,m_MessageFactory,m_MessageProcessor);
			lock(m_SyncRoot)
			{
				m_Processors.Add(processor.ID,processor);
			}

			processor.Start();
		}

		private void HandleMessage(object sender, SocketMessageEventArgs<MessageEnvelope,byte[]> args)
		{
			m_CallDispatcher.QueueUserWorkItem(s=>DispatchCall(args.Header,args.Body));

			args.ReadMode=ReadMode.KeepReading;
		}

		private void HandleDisconnect(object sender, SocketProcessorEventArgs args)
		{
			var processor=args.SocketProcessor;

			lock(m_SyncRoot)
			{
				m_Processors.Remove(processor.ID);
			}

			processor.Close();
		}

		private void HandleNetworkFault(object sender, SocketProcessorEventArgs args)
		{
			var processor=args.SocketProcessor;

			lock(m_SyncRoot)
			{
				m_Processors.Remove(processor.ID);
			}

			processor.Close();
		}

		private void DispatchCall(MessageEnvelope envelope, byte[] data)
		{
			var args=new ServiceCallEventArgs(this,envelope,data);
			OnServiceCall(args);
		}

		public override void Respond(MessageEnvelope requestMessageEnvelope, IList<ArraySegment<byte>> buffers)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return m_Endpoint.ToString();
		}

		private static IPAddress ResolveIPAddress(string name)
		{
			IPAddress address=null;
			if(IPAddress.TryParse(name,out address)) return address;

			var hostAddresses=Dns.GetHostAddresses(name);
			foreach(var hostAddress in hostAddresses)
			{
				if(hostAddress.AddressFamily==AddressFamily.InterNetwork)
				{
					return hostAddress;
				}
			}

			return null;
		}
	}
}
