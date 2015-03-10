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
	public abstract class FixedHeaderSocketProcessor<THeader,TBody> : SocketProcessor, IDisposable
	{
		private Socket m_Socket;
		private NetworkStream m_Stream;

		private long m_Disposed;		

		public FixedHeaderSocketProcessor(Socket socket)
		{
			if(socket==null) throw new ArgumentNullException("socket");

			m_Socket=socket;
			m_Stream=new NetworkStream(socket);
		}

		protected abstract int HeaderSize{get;}

		protected abstract THeader CreateHeader(byte[] buffer);
		protected abstract TBody CreateBody(THeader header, byte[] buffer);
		protected abstract int GetBodySize(THeader header);

		public void Start(Action<SocketProcessor,SocketProcessorResult<THeader,TBody>> handler)
		{
			Read(handler);
		}

		private void Read(Action<SocketProcessor,SocketProcessorResult<THeader,TBody>> handler)
		{
			while(this.KeepReading)
			{
				State state=new State();
				state.Handler=handler;

				int headerSize=this.HeaderSize;
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

					if(state.HeaderOffset!=this.HeaderSize)
					{
						BeginReadHeader(state);
					}
					else
					{
						state.Header=CreateHeader(state.HeaderBuffer);
						int bodySize=GetBodySize(state.Header);
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
						state.Body=CreateBody(state.Header,state.BodyBuffer);
						state.Handler(this,new SocketProcessorResult<THeader,TBody>(state.Header,state.Body));

						Read(state.Handler);
					}
				}
			});
		}

		private void HandleNetworkCall(Action action)
		{
			// TODO: Move to base

			try
			{
				action();
			}
			catch
			{
				if(Interlocked.Read(ref m_Disposed)!=1)
				{
					OnNetworkFault(EventArgs.Empty);
				}
			}
		}

		public void Dispose()
		{
			if(m_Socket!=null)
			{
				Interlocked.Exchange(ref m_Disposed,1);

				MethodCall.AllowFail(()=>m_Stream.Close());
				MethodCall.AllowFail(()=>m_Socket.Close());

				m_Socket=null;
			}
		}

		class State
		{
			public Action<SocketProcessor,SocketProcessorResult<THeader,TBody>> Handler;

			public byte[] HeaderBuffer;
			public int HeaderOffset;
			public THeader Header;

			public byte[] BodyBuffer;
			public int BodyOffset;
			public TBody Body;
		}
	}
}
