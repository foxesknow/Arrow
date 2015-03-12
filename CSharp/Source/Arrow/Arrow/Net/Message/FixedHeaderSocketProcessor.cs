using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

using Arrow.Execution;

namespace Arrow.Net.Message
{
	/// <summary>
	/// A socket processor that reads a fixes sized header and an arbitary body
	/// </summary>
	/// <typeparam name="THeader">The type of the header</typeparam>
	/// <typeparam name="TBody">The type of the body</typeparam>
	public class FixedHeaderSocketProcessor<THeader,TBody> : SocketProcessor, IDisposable
	{
		private readonly Socket m_Socket;

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

			//m_Stream=new NetworkStream(socket);
		}

		/// <summary>
		/// Starts the asynchronous read loop
		/// </summary>
		public override void Start()
		{
			int headerSize=m_MessageFactory.HeaderSize;
			State state=new State(headerSize);

			BeginReadHeader(state);
		}

		/// <summary>
		/// Writes data to the socket
		/// </summary>
		/// <param name="buffer">The data to write</param>
		/// <param name="offset">The start of the data within the buffer</param>
		/// <param name="size">How much data to write</param>
		public override void Write(byte[] buffer, int offset, int size)
		{
			if(buffer==null) throw new ArgumentNullException("buffer");

			bool success=HandleNetworkCall(()=>
			{
				m_Socket.Send(buffer,offset,size,SocketFlags.None);
			});

			if(success==false) throw new IOException("write failed");
		}

		/// <summary>
		/// Writes data to the socket
		/// </summary>
		/// <param name="buffer">The data to write</param>
		/// <param name="offset">The start of the data within the buffer</param>
		/// <param name="size">How much data to write</param>
		/// <returns>A task that will be signalled when the write completes</returns>
		public override Task<SocketProcessor> WriteAsync(byte[] buffer, int offset, int size)
		{
			if(buffer==null) throw new ArgumentNullException("buffer");

			var completionSource=new TaskCompletionSource<SocketProcessor>();

			bool success=HandleNetworkCall(()=>
			{
				m_Socket.BeginSend(buffer,offset,size,SocketFlags.None,ar=>EndWrite(ar,completionSource),null);
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
				m_Socket.EndSend(result);
				completionSource.SetResult(this);
				success=true;
			});

			if(success==false)
			{
				completionSource.SetException(new IOException("write failed"));
			}
		}

		/// <summary>
		/// Closes the socket processor
		/// </summary>
		public override void Close()
		{
			if(IsClosed()==false)
			{
				/*
				 * We're going to start closing the stream and the socket.
				 * However, they may have pending read/writes on them, so we need to 
				 * make sure we know we're closing to handle any exceptions we get
				 * from their Read/Write methods
				 */
				FlagAsClosed();

				MethodCall.AllowFail(()=>m_Socket.Dispose());
			}
		}

		private void BeginReadHeader(State state)
		{
			HandleNetworkCall(()=>
			{
				m_Socket.BeginReceive
				(
					state.HeaderBuffer,
					state.HeaderOffset,
					state.HeaderBuffer.Length-state.HeaderOffset,
					SocketFlags.None,
					ar=>EndReadHeader(ar,state),
					null
				);
			});
		}

		private void EndReadHeader(IAsyncResult result, State state)
		{
			HandleNetworkCall(()=>
			{
				int bytesRead=m_Socket.EndReceive(result);

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
				m_Socket.BeginReceive
				(
					state.BodyBuffer,
					state.BodyOffset,
					state.BodyBuffer.Length-state.BodyOffset,
					SocketFlags.None,
					ar=>EndReadBody(ar,state),
					null
				);
			});
		}

		private void EndReadBody(IAsyncResult result, State state)
		{
			HandleNetworkCall(()=>
			{
				int bytesRead=m_Socket.EndReceive(result);

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
							state.Reset();
							BeginReadHeader(state);
						}
					}
				}
			});
		}

		/// <summary>
		/// Handles any disconnection notifications
		/// </summary>
		protected override void OnDisconnected()
		{
			m_MessageProcessor.HandleDisconnect(this);
		}		

		/// <summary>
		/// Disposes of the processor
		/// </summary>
		public void Dispose()
		{
			Close();
		}

		class State
		{
			public readonly byte[] HeaderBuffer;
			public int HeaderOffset;
			public THeader Header;

			public byte[] BodyBuffer;
			public int BodyOffset;
			public TBody Body;

			public State(int headerSize)
			{
				this.HeaderBuffer=new byte[headerSize];
			}

			public void Reset()
			{
				// We can keep the header buffer as its size won't change
				// but we need to reset the rest
				this.HeaderOffset=0;
				this.Header=default(THeader);

				this.BodyBuffer=null;
				this.BodyOffset=0;
				this.Body=default(TBody);
			}
		}
	}
}
