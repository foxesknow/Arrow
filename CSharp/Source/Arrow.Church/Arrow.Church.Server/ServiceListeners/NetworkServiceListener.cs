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
using Arrow.Church.Common.Data;
using System.IO;
using Arrow.Logging;
using System.Threading;

namespace Arrow.Church.Server.ServiceListeners
{
	public partial class NetworkServiceListener : ServiceListener
	{
		private static readonly ILog Log=LogManager.GetDefaultLog();

		private readonly object m_SyncRoot=new object();
		private readonly Dictionary<long,SocketProcessor> m_Processors=new Dictionary<long,SocketProcessor>();
		private long m_Closed;

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
			m_TcpListener.Start();

			m_TcpListener.BeginAcceptSocket(HandleAcceptSocket,null);
		}

		private void HandleAcceptSocket(IAsyncResult result)
		{
			try
			{
				var socket=m_TcpListener.EndAcceptSocket(result);

				
				lock(m_SyncRoot)
				{
					if(IsClosed()==false)
					{
						var processor=new FixedHeaderSocketProcessor<MessageEnvelope,byte[]>(socket,m_MessageFactory,m_MessageProcessor);
						m_Processors.Add(processor.ID,processor);
						processor.Start();
					}
				}

				// Recurse to keep on accepting
				m_TcpListener.BeginAcceptSocket(HandleAcceptSocket,null);
			}
			catch(Exception e)
			{
				if(IsClosed())
				{
					Log.Info("HandleAcceptSocket - exception due to closing");
				}
				else
				{
					Log.ErrorFormat("HandleAcceptSocket - failed {0}",e);
				}
			}
		}

		private void HandleMessage(object sender, SocketMessageEventArgs<MessageEnvelope,byte[]> args)
		{
			m_CallDispatcher.QueueUserWorkItem(s=>DispatchCall(args.Header,args.Body));

			args.ReadMode=ReadMode.KeepReading;
		}

		private void HandleDisconnect(object sender, SocketProcessorEventArgs args)
		{
			if(IsClosed()) return;

			var processor=args.SocketProcessor;

			lock(m_SyncRoot)
			{
				m_Processors.Remove(processor.ID);
			}

			processor.Close();
		}

		private void HandleNetworkFault(object sender, SocketProcessorEventArgs args)
		{
			if(IsClosed()) return;

			var processor=args.SocketProcessor;

			lock(m_SyncRoot)
			{
				m_Processors.Remove(processor.ID);
			}

			processor.Close();
		}

		private void DispatchCall(MessageEnvelope envelope, byte[] data)
		{
			var callID=AllocateCallID();
			var callDetails=new CallDetails(envelope,data,callID);

			var args=new ServiceCallEventArgs(this,callDetails);
			OnServiceCall(args);
		}

		public override void Respond(CallDetails callDetails, IList<ArraySegment<byte>> buffers)
		{
			SocketProcessor processor=null;

			lock(m_SyncRoot)
			{
				m_Processors.TryGetValue(callDetails.CallID,out processor);
			}

			if(processor!=null)
			{
				// We've already got the buffers for the response,
				// but we need to flatten the envelope and stick it on the front
				var envelope=CreateReponse(callDetails.RequestMessageEnvelope);
				envelope.DataLength=buffers.TotalLength();
				var response=GenerateResponse(envelope,buffers);

				try
				{
					var writeTask=processor.WriteAsync(response);
					writeTask.ContinueWith(WriteResponseComplete);
				}
				catch(Exception e)
				{
					Log.ErrorFormat("Respond - failed to send reponse: {0}",e);
				}
			}
		}

		public void Close()
		{
			if(FlagAsClosed()) 
			{
				// We're already flagged, so no need to do it again
				return;
			}

			m_TcpListener.Stop();

			lock(m_SyncRoot)
			{
				foreach(var processor in m_Processors.Values)
				{
					processor.Close();
				}

				m_Processors.Clear();
			}
		}

		/// <summary>
		/// Flags the network system as being closed.
		/// This allows the processor to work out how to handle exceptions thrown by network calls
		/// </summary>
		protected bool FlagAsClosed()
		{
			var originalValue=Interlocked.Exchange(ref m_Closed,1);
			return originalValue!=0;
		}

		/// <summary>
		/// Checks to see if the socket processor is closed
		/// </summary>
		/// <returns></returns>
		protected bool IsClosed()
		{
			return Interlocked.Read(ref m_Closed)==1;
		}

		private void WriteResponseComplete(Task<WriteResults> results)
		{
			// TODO: Better handle this
			if(results.IsFaulted)
			{
				Log.ErrorFormat("WriteResponseComplete - failed to send data to caller: {0}",results.Exception);
			}
			else
			{
				Log.InfoFormat("WriteResponseComplete - success: {0}",results.Result);
			}
		}

		private IList<ArraySegment<byte>> GenerateResponse(MessageEnvelope envelope, IList<ArraySegment<byte>> buffers)
		{
			using(var stream=new MemoryStream(MessageEnvelope.EnvelopeSize))
			{
				using(var encoder=new DataEncoder(stream))
				{
					encoder.Write(envelope);
				}

				var segment=stream.ToArraySegment();

				List<ArraySegment<byte>> responseBuffers=new List<ArraySegment<byte>>(1+buffers.Count);
				responseBuffers.Add(segment);
				responseBuffers.AddRange(buffers);

				return responseBuffers;
			}
		}

		public override void Dispose()
		{
			Close();
			base.Dispose();			
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
