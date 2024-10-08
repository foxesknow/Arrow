﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

using Arrow.Church.Common.Net;
using Arrow.Net.Message;
using Arrow.Threading;
using Arrow.Church.Common.Data;
using Arrow.Church.Common.Internal;
using Arrow.Logging;
using Arrow.Church.Common;
using Arrow.Memory;


namespace Arrow.Church.Server.ServiceListeners
{
	public partial class NetworkServiceListener : ServiceListener
	{
		private static readonly ILog Log=LogManager.GetDefaultLog();

		private readonly object m_SyncRoot=new object();
		private readonly Dictionary<long,SocketProcessor> m_Processors=new Dictionary<long,SocketProcessor>();
		private long m_Stopped;

		private readonly IWorkDispatcher m_CallDispatcher=new ThreadPoolWorkDispatcher();

		private readonly IPAddress m_Address;
		private readonly TcpListener m_TcpListener;
		
		private readonly EventMessageProcessor<MessageEnvelope,byte[]> m_MessageProcessor=new EventMessageProcessor<MessageEnvelope,byte[]>();
		private readonly IMessageFactory<MessageEnvelope,byte[]> m_MessageFactory=new MessageEnvelopeMessageFactory();

		public NetworkServiceListener(Uri endpoint) : base(endpoint)
		{			
			m_Address=endpoint.TryResolveIPAddress();
			if(m_Address==null) throw new ArgumentException("could not resolve host: "+endpoint.Host.ToString(),"endpoint");

			m_MessageProcessor.Message+=HandleMessage;
			m_MessageProcessor.Disconnect+=HandleDisconnect;
			m_MessageProcessor.NetworkFault+=HandleNetworkFault;

			m_TcpListener=new TcpListener(m_Address,endpoint.Port);			
		}

		public override void Start()
		{
			if(IsStopped())
			{
				m_TcpListener.Start();
				m_TcpListener.BeginAcceptSocket(HandleAcceptSocket,null);
				FlagAsStarted();
			}
		}

		public override void Stop()
		{
			if(FlagAsStopped())
			{
				m_TcpListener.Stop();
			}
		}

		private void HandleAcceptSocket(IAsyncResult result)
		{
			try
			{
				// This method will be triggered if the listener is closing down
				// so check to save throwing an exception...
				if(IsStopped()) return;

				// ...However, it's possible that we're not stopping but something goes wrong
				// with the socket here, so play safe
				var socket=m_TcpListener.EndAcceptSocket(result);

				
				lock(m_SyncRoot)
				{
					if(IsStopped()==false)
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
				if(IsStopped())
				{
					Log.Info("NetworkServiceListener.HandleAcceptSocket - exception due to stopping");
				}
				else
				{
					Log.ErrorFormat("NetworkServiceListener.HandleAcceptSocket - failed {0}",e);
				}
			}
		}

		private void HandleMessage(object sender, SocketMessageEventArgs<MessageEnvelope,byte[]> args)
		{
			var callDetails=new CallDetails(args.Header,args.Body,args.SocketProcessor.ID);
			
			m_CallDispatcher.QueueUserWorkItem(cd=>
			{
				HandleMessage((CallDetails)cd);
			},callDetails);

			args.ReadMode=ReadMode.KeepReading;
		}

		private void HandleDisconnect(object sender, SocketProcessorEventArgs args)
		{
			if(IsStopped()) return;

			var processor=args.SocketProcessor;

			lock(m_SyncRoot)
			{
				m_Processors.Remove(processor.ID);
			}

			processor.Close();
		}

		private void HandleNetworkFault(object sender, SocketProcessorEventArgs args)
		{
			if(IsStopped()) return;

			var processor=args.SocketProcessor;

			lock(m_SyncRoot)
			{
				m_Processors.Remove(processor.ID);
			}

			processor.Close();
		}

		public override Task RespondAsync(CallDetails callDetails, ArraySegmentCollection<byte> buffers)
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
				var envelope=CreateReponse(callDetails.Envelope);
				envelope.DataLength=buffers.GetOverallLength();
				GenerateResponse(envelope,buffers);

				return processor.WriteAsync(buffers.UnderlyingSegments);
			}
			else
			{
				return TaskEx.FromException(new ChurchException("could not find socket processor"));
			}
		}

		public void Close()
		{
			if(FlagAsStopped()) 
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
		protected bool FlagAsStopped()
		{
			var originalValue=Interlocked.Exchange(ref m_Stopped,0);
			return originalValue==1;
		}

		protected void FlagAsStarted()
		{
			Interlocked.Exchange(ref m_Stopped,1);
		}

		/// <summary>
		/// Checks to see if the socket processor is closed
		/// </summary>
		/// <returns></returns>
		protected bool IsStopped()
		{
			return Interlocked.Read(ref m_Stopped)==0;
		}

		private void WriteResponseComplete(Task<WriteResults> results)
		{
			// TODO: Better handle this
			if(results.IsFaulted)
			{
				Log.ErrorFormat("NetworkServiceListener.WriteResponseComplete - failed to send data to caller: {0}",results.Exception);
			}
			else
			{
				Log.InfoFormat("NetworkServiceListener.WriteResponseComplete - success: {0}",results.Result);
			}
		}

		private void GenerateResponse(MessageEnvelope envelope, ArraySegmentCollection<byte> buffers)
		{
			using(var stream=new MemoryStream(MessageEnvelope.EnvelopeSize))
			{
				using(var encoder=new DataEncoder(stream))
				{
					encoder.WriteNeverNull(envelope);
				}

				var segment=stream.ToArraySegment();
				
				// Add to the front as the envelope preceeds the message body
				buffers.AddFront(segment);
			}
		}

		public override void Dispose()
		{
			Close();
			base.Dispose();			
		}	
	}
}
