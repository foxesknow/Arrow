using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Arrow.Execution;

namespace Arrow.Net
{
	public class FixedHeaderSocketProcessor<THeader,TBody> : SocketProcessor, IDisposable
	{
		private Socket m_Socket;
		private NetworkStream m_Stream;

		private readonly IMessageFactory<THeader,TBody> m_MessageFactory;
		private readonly IMessageProcessor<THeader,TBody> m_MessageProcessor;

		private long m_Disposed;		

		public FixedHeaderSocketProcessor(Socket socket, IMessageFactory<THeader,TBody> messageFactory, IMessageProcessor<THeader,TBody> messageProcessor)
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
		public void Start()
		{
			Read();
		}

		public override Task<SocketProcessor> Write(byte[] buffer, int offset, int size)
		{
			if(buffer==null) throw new ArgumentNullException("buffer");

			var completionSource=new TaskCompletionSource<SocketProcessor>();

			bool success=HandleNetworkCall(()=>
			{
				m_Stream.BeginWrite(buffer,offset,size,ar=>EndWrite(ar,completionSource),null);
			});

			if(success==false)
			{
				completionSource.SetException(new OperationCanceledException("write failed"));
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
				completionSource.SetException(new OperationCanceledException("write failed"));
			}
		}

		public override void Close()
		{
			if(m_Socket!=null)
			{
				Interlocked.Exchange(ref m_Disposed,1);

				MethodCall.AllowFail(()=>m_Stream.Close());
				MethodCall.AllowFail(()=>m_Socket.Close());

				m_Socket=null;
			}
		}

		private void Read()
		{
			while(this.KeepReading)
			{
				State state=new State();

				int headerSize=m_MessageFactory.HeaderSize;
				state.HeaderBuffer=new byte[headerSize];
				state.HeaderOffset=0;

				BeginReadHeader(state);
			}
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
						var readMode=m_MessageProcessor.Process(this,state.Header,state.Body);

						if(readMode==ReadMode.KeepReading)
						{
							Read();
						}
					}
				}
			});
		}

		private bool HandleNetworkCall(Action action)
		{
			// TODO: Move to base

			try
			{
				action();
				return true;
			}
			catch
			{
				if(Interlocked.Read(ref m_Disposed)!=1)
				{
					m_MessageProcessor.HandleNetworkFault(this);
				}
			}

			return false;
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
