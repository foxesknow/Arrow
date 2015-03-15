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

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="socket">The socket to process</param>
		/// <param name="messageFactory">The message factory class that will create the header and bodies</param>
		/// <param name="messageProcessor">The message processor that will handle the messages</param>
		public FixedHeaderSocketProcessor(Socket socket, IMessageFactory<THeader,TBody> messageFactory, IMessageProcessor<THeader,TBody> messageProcessor) : base(socket,messageProcessor)
		{
			if(messageFactory==null) throw new ArgumentNullException("messageFactory");
			if(messageProcessor==null) throw new ArgumentNullException("messageProcessor");

			m_MessageFactory=messageFactory;
			m_MessageProcessor=messageProcessor;
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
			HandleNetworkCall(state,(localState)=>
			{
				this.Socket.BeginReceive
				(
					state.HeaderBuffer,
					state.HeaderOffset,
					state.HeaderBuffer.Length-state.HeaderOffset,
					SocketFlags.None,
					EndReadHeader,
					localState
				);
			});
		}

		private void EndReadHeader(IAsyncResult result)
		{
			HandleNetworkCall(result,(ar)=>
			{
				int bytesRead=this.Socket.EndReceive(ar);

				if(CanProcessBytesRead(bytesRead))
				{
					State state=(State)ar.AsyncState;
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
			HandleNetworkCall(state,(localState)=>
			{
				this.Socket.BeginReceive
				(
					state.BodyBuffer,
					state.BodyOffset,
					state.BodyBuffer.Length-state.BodyOffset,
					SocketFlags.None,
					EndReadBody,
					localState
				);
			});
		}

		private void EndReadBody(IAsyncResult result)
		{
			HandleNetworkCall(result,(ar)=>
			{
				int bytesRead=this.Socket.EndReceive(ar);

				if(CanProcessBytesRead(bytesRead))
				{
					State state=(State)ar.AsyncState;
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
			});
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

				this.BodyBuffer=null;
				this.BodyOffset=0;
				this.Body=default(TBody);
			}
		}
	}
}
