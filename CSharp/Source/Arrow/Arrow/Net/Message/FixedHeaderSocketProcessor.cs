using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Arrow.Execution;
using System.IO;

namespace Arrow.Net.Message
{
	/// <summary>
	/// A socket processor that reads a fixes sized header and an arbitary body
	/// </summary>
	/// <typeparam name="THeader">The type of the header</typeparam>
	/// <typeparam name="TBody">The type of the body</typeparam>
	public class FixedHeaderSocketProcessor<THeader,TBody> : SocketProcessor, IDisposable
	{
		private Socket m_Socket;
		private NetworkStream m_Stream;

		private readonly IMessageFactory<THeader,TBody> m_MessageFactory;
		private readonly IMessageProcessor<THeader,TBody> m_MessageProcessor;		

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="socket">The socket to process</param>
		/// <param name="messageFactory">The message factory class that will create the header and bodies</param>
		/// <param name="messageProcessor">The message processor that will handle the messages</param>
		public FixedHeaderSocketProcessor(Socket socket, IMessageFactory<THeader,TBody> messageFactory, IMessageProcessor<THeader,TBody> messageProcessor) : base(messageProcessor)
		{
			if(socket==null) throw new ArgumentNullException("socket");
			if(messageFactory==null) throw new ArgumentNullException("messageFactory");
			if(messageProcessor==null) throw new ArgumentNullException("messageProcessor");

			m_Socket=socket;
			m_MessageFactory=messageFactory;
			m_MessageProcessor=messageProcessor;

			m_Stream=new NetworkStream(socket);
		}

		/// <summary>
		/// Starts the asynchronous read loop
		/// </summary>
		public override void Start()
		{
			Read();
		}

		public override void Write(byte[] buffer, int offset, int size)
		{
			if(buffer==null) throw new ArgumentNullException("buffer");

			bool success=HandleNetworkCall(()=>
			{
				m_Stream.Write(buffer,offset,size);
			});

			if(success==false) throw new IOException("write failed");
		}

		public override Task<SocketProcessor> WriteAsync(byte[] buffer, int offset, int size)
		{
			if(buffer==null) throw new ArgumentNullException("buffer");

			var completionSource=new TaskCompletionSource<SocketProcessor>();

			bool success=HandleNetworkCall(()=>
			{
				m_Stream.BeginWrite(buffer,offset,size,ar=>EndWrite(ar,completionSource),null);
			});

			if(success==false)
			{
				completionSource.SetException(new IOException("write failed"));
			}

			return completionSource.Task;
		}

		private void EndWrite(IAsyncResult result, TaskCompletionSource<SocketProcessor> completionSource)
		{
			bool success=HandleNetworkCall(()=>
			{
				m_Stream.EndWrite(result);
				completionSource.SetResult(this);
				success=true;
			});

			if(success==false)
			{
				completionSource.SetException(new IOException("write failed"));
			}
		}

		public override void Close()
		{
			if(m_Socket!=null)
			{
				/*
				 * We're going to start closing the stream and the socket.
				 * However, they may have pending read/writes on them, so we need to 
				 * make sure we know we're closing to handle any exceptions we get
				 * from their Read/Write methods
				 */
				FlagAsClosed();

				MethodCall.AllowFail(()=>m_Stream.Close());
				MethodCall.AllowFail(()=>m_Socket.Close());

				m_Socket=null;
			}
		}

		private void Read()
		{
			State state=new State();

			int headerSize=m_MessageFactory.HeaderSize;
			state.HeaderBuffer=new byte[headerSize];
			state.HeaderOffset=0;

			BeginReadHeader(state);
		}

		private void BeginReadHeader(State state)
		{
			HandleNetworkCall(()=>
			{
				m_Stream.BeginRead(state.HeaderBuffer,state.HeaderOffset,state.HeaderBuffer.Length-state.HeaderOffset,ar=>EndReadHeader(ar,state),null);
			});
		}

		private void EndReadHeader(IAsyncResult result, State state)
		{
			HandleNetworkCall(()=>
			{
				int bytesRead=m_Stream.EndRead(result);

				if(CanProcessBytesRead(bytesRead))
				{
					state.HeaderOffset+=bytesRead;

					if(state.HeaderOffset!=m_MessageFactory.HeaderSize)
					{
						BeginReadHeader(state);
					}
					else
					{
						state.Header=m_MessageFactory.CreateHeader(state.HeaderBuffer);
						int bodySize=m_MessageFactory.GetBodySize(state.Header);
						state.BodyBuffer=new byte[bodySize];
						
						// We don't need the header buffer any more, so it's safe to discard it
						state.HeaderBuffer=null;
						
						BeginReadBody(state);
					}
				}
			});
		}

		private void BeginReadBody(State state)
		{
			HandleNetworkCall(()=>
			{
				m_Stream.BeginRead(state.BodyBuffer,state.BodyOffset,state.BodyBuffer.Length-state.BodyOffset,ar=>EndReadBody(ar,state),null);
			});
		}

		private void EndReadBody(IAsyncResult result, State state)
		{
			HandleNetworkCall(()=>
			{
				int bytesRead=m_Stream.EndRead(result);

				if(CanProcessBytesRead(bytesRead))
				{
					state.BodyOffset+=bytesRead;

					if(state.BodyOffset!=state.BodyBuffer.Length)
					{
						BeginReadBody(state);
					}
					else
					{
						state.Body=m_MessageFactory.CreateBody(state.Header,state.BodyBuffer);

						// We don't need the body buffer any more so discard it...
						state.BodyBuffer=null;

						// ...and now process the actual message
						var readMode=m_MessageProcessor.Process(this,state.Header,state.Body);

						if(readMode==ReadMode.KeepReading)
						{
							Read();
						}
					}
				}
			});
		}

		protected override void OnDisconnected()
		{
			m_MessageProcessor.HandleDisconnect(this);
		}		

		public void Dispose()
		{
			Close();
		}

		class State
		{
			public byte[] HeaderBuffer;
			public int HeaderOffset;
			public THeader Header;

			public byte[] BodyBuffer;
			public int BodyOffset;
			public TBody Body;
		}
	}
}
