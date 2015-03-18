using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace Arrow.Net.Message
{
	/// <summary>
	/// A socket processor that reads a fixes sized header and an arbitary body
	/// </summary>
	/// <typeparam name="THeader">The type of the header</typeparam>
	/// <typeparam name="TBody">The type of the body</typeparam>
	public class FixedHeaderSocketProcessor<THeader,TBody> : SocketProcessor
	{
		private readonly IMessageFactory<THeader,TBody> m_MessageFactory;
		private readonly IMessageProcessor<THeader,TBody> m_MessageProcessor;		

		private readonly Action<State> m_HandleBeginReadHeader;
		private readonly AsyncCallback m_EndReadHeader;
		private readonly Action<IAsyncResult> m_HandleEndReadHeader;

		private readonly Action<State> m_HandleBeginReadBody;
		private readonly AsyncCallback m_EndReadBody;
		private readonly Action<IAsyncResult> m_HandleEndReadBody;

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="socket">The socket to process</param>
		/// <param name="messageFactory">The message factory class that will create the header and bodies</param>
		/// <param name="messageProcessor">The message processor that will handle the messages</param>
		public FixedHeaderSocketProcessor(Socket socket, IMessageFactory<THeader,TBody> messageFactory, IMessageProcessor<THeader,TBody> messageProcessor) : base(socket,messageProcessor)
		{
			if(messageFactory==null) throw new ArgumentNullException("messageFactory");

			m_MessageFactory=messageFactory;
			m_MessageProcessor=messageProcessor;

			m_HandleBeginReadHeader=HandleBeginReadHeader;
			m_EndReadHeader=EndReadHeader;
			m_HandleEndReadHeader=HandleEndReadHeader;

			m_HandleBeginReadBody=HandleBeginReadBody;
			m_EndReadBody=EndReadBody;
			m_HandleEndReadBody=HandleEndReadBody;
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

		private void BeginReadHeader(State state)
		{
			HandleNetworkCall(state,m_HandleBeginReadHeader);
		}

		private void HandleBeginReadHeader(State state)
		{
			this.Socket.BeginReceive
			(
				state.HeaderBuffer,
				state.HeaderOffset,
				state.HeaderBuffer.Length-state.HeaderOffset,
				SocketFlags.None,
				m_EndReadHeader,
				state
			);
		}

		private void EndReadHeader(IAsyncResult result)
		{
			HandleNetworkCall(result,m_HandleEndReadHeader);
		}

		private void HandleEndReadHeader(IAsyncResult result)
		{
			int bytesRead=this.Socket.EndReceive(result);

			if(CanProcessBytesRead(bytesRead))
			{
				State state=(State)result.AsyncState;
				state.HeaderOffset+=bytesRead;

				if(state.HeaderOffset!=m_MessageFactory.HeaderSize)
				{
					BeginReadHeader(state);
				}
				else
				{
					state.Header=m_MessageFactory.CreateHeader(state.HeaderBuffer);
					int bodySize=m_MessageFactory.GetBodySize(state.Header);					
					state.AllocateBodyBuffer(bodySize);
						
					BeginReadBody(state);
				}
			}
		}

		private void BeginReadBody(State state)
		{
			HandleNetworkCall(state,m_HandleBeginReadBody);
		}

		private void HandleBeginReadBody(State state)
		{
			this.Socket.BeginReceive
			(
				state.BodyBuffer,
				state.BodyOffset,
				state.BodyBuffer.Length-state.BodyOffset,
				SocketFlags.None,
				m_EndReadBody,
				state
			);
		}

		private void EndReadBody(IAsyncResult result)
		{
			HandleNetworkCall(result,m_HandleEndReadBody);
		}

		private void HandleEndReadBody(IAsyncResult result)
		{
			int bytesRead=this.Socket.EndReceive(result);

			if(CanProcessBytesRead(bytesRead))
			{
				State state=(State)result.AsyncState;
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
					var readMode=m_MessageProcessor.ProcessMessage(this,state.Header,state.Body);

					if(readMode==ReadMode.KeepReading)
					{
						state.Reset();
						BeginReadHeader(state);
					}
				}
			}
		}

		/// <summary>
		/// Handles any disconnection notifications
		/// </summary>
		protected override void OnDisconnected()
		{
			m_MessageProcessor.HandleDisconnect(this);
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

				//this.BodyBuffer=null;
				this.BodyOffset=0;
				this.Body=default(TBody);
			}

			public void AllocateBodyBuffer(int size)
			{
				this.BodyBuffer=new byte[size];

				/*
				 * NOTE: We could keep the buffer if the new size is less than the current size.
				 * However, if a message is huge (say 500MB) and then all subsequent ones are less
				 * then we'll never release the memory, which will cause problems over time.
				 * 
				if(this.BodyBuffer==null || this.BodyBuffer.Length<size)
				{
					this.BodyBuffer=new byte[size];
				}
				*/
			}
		}
	}
}
