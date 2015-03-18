using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.IO;

using Arrow.Execution;

namespace Arrow.Net.Message
{
	/// <summary>
	/// Base class for all socket processors
	/// </summary>
	public abstract class SocketProcessor : IDisposable
	{
		private static long s_ID=0;

		private readonly Socket m_Socket;
		private readonly IMessageProcessor m_MessageProcessor;

		private long m_Closed;
		private readonly long m_ID;

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="socket">The socket to communicate over</param>
		/// <param name="messageProcessor">The message processor to use</param>
		protected SocketProcessor(Socket socket, IMessageProcessor messageProcessor)
		{
			if(socket==null) throw new ArgumentNullException("socket");
			if(messageProcessor==null) throw new ArgumentNullException("messageProcessor");

			m_Socket=socket;
			m_MessageProcessor=messageProcessor;
			m_ID=Interlocked.Increment(ref s_ID);
		}

		/// <summary>
		/// Returns a unique id for the connection
		/// </summary>
		public long ID
		{
			get{return m_ID;}
		}

		/// <summary>
		/// The socket being used
		/// </summary>
		protected Socket Socket
		{
			get{return m_Socket;}
		}

		/// <summary>
		/// Determines if the processor should deal with the bytes read from a socket.
		/// This is typically called on an EndRead to determine if the socket was connected
		/// </summary>
		/// <param name="bytesRead">The number of bytes read</param>
		/// <returns>true if the caller should handle the bytes, otherwise false</returns>
		protected bool CanProcessBytesRead(int bytesRead)
		{
			if(bytesRead==0)
			{
				OnDisconnected();
				return false;
			}
			else
			{
				return true;
			}
		}

		/// <summary>
		/// Flags the network system as being closed.
		/// This allows the processor to work out how to handle exceptions thrown by network calls
		/// </summary>
		protected void FlagAsClosed()
		{
			Interlocked.Exchange(ref m_Closed,1);
		}

		/// <summary>
		/// Checks to see if the socket processor is closed
		/// </summary>
		/// <returns></returns>
		protected bool IsClosed()
		{
			return Interlocked.Read(ref m_Closed)==1;
		}

		/// <summary>
		/// Executes a piece of network code and deals with any errors
		/// </summary>
		/// <param name="action">The network code to execute</param>
		/// <returns>true if the network code executes successfully, otherwise false</returns>
		protected bool HandleNetworkCall(Action action)
		{
			try
			{
				action();
				return true;
			}
			catch
			{
				if(Interlocked.Read(ref m_Closed)!=1)
				{
					m_MessageProcessor.HandleNetworkFault(this);
				}
			}

			return false;
		}

		/// <summary>
		/// Executes a piece of network code and deals with any errors
		/// </summary>
		/// <param name="state">State to pass into the action</param>
		/// <param name="action">The network code to execute</param>
		/// <returns>true if the network code executes successfully, otherwise false</returns>
		protected bool HandleNetworkCall<T>(T state, Action<T> action)
		{
			try
			{
				action(state);
				return true;
			}
			catch
			{
				if(Interlocked.Read(ref m_Closed)!=1)
				{
					m_MessageProcessor.HandleNetworkFault(this);
				}
			}

			return false;
		}

		/// <summary>
		/// Starts reading messages from the socket
		/// </summary>
		public abstract void Start();

		/// <summary>
		/// Called when a network disconnect is detected
		/// </summary>
		protected abstract void OnDisconnected();
		

		/// <summary>
		/// Writes data to the socket
		/// </summary>
		/// <param name="buffer">The data to write</param>
		/// <param name="offset">The start of the data within the buffer</param>
		/// <param name="size">How much data to write</param>
		/// <returns>The number of bytes written</returns>
		public int Write(byte[] buffer, int offset, int size)
		{
			if(buffer==null) throw new ArgumentNullException("buffer");

			int bytesWritten=0;
			bool success=HandleNetworkCall(()=>
			{
				bytesWritten=this.Socket.Send(buffer,offset,size,SocketFlags.None);
			});

			if(success==false) throw new IOException("Write failed");
			return bytesWritten;
		}

		/// <summary>
		/// Writes a series of buffers to the socket
		/// </summary>
		/// <param name="buffers">The buffers to write</param>
		/// <returns>The number of bytes written</returns>
		public int Write(IList<ArraySegment<byte>> buffers)
		{
			if(buffers==null) throw new ArgumentNullException("buffers");

			int bytesWritten=0;			
			bool success=HandleNetworkCall(buffers,b=>
			{
				bytesWritten=this.Socket.Send(buffers);
			});

			if(success==false) throw new IOException("Write failed");			
			return bytesWritten;
		}


		/// <summary>
		/// Asynchronously writes data to the socket
		/// </summary>
		/// <param name="buffer">The data to write</param>
		/// <param name="offset">The start of the data within the buffer</param>
		/// <param name="size">How much data to write</param>
		/// <returns>A task that will be signalled when the write completes</returns>
		public Task<WriteResults> WriteAsync(byte[] buffer, int offset, int size)
		{
			if(buffer==null) throw new ArgumentNullException("buffer");

			var completionSource=new TaskCompletionSource<WriteResults>();

			bool success=HandleNetworkCall(()=>
			{
				this.Socket.BeginSend(buffer,offset,size,SocketFlags.None,EndWrite,completionSource);
			});

			if(success==false)
			{
				completionSource.SetException(new IOException("WriteAysnc failed"));
			}

			return completionSource.Task;
		}

		/// <summary>
		/// Asynchronously writes a series of buffer to the socket
		/// </summary>
		/// <param name="buffers">The buffers to write</param>
		/// <returns>A task that will be signalled when the write completes</returns>
		public Task<WriteResults> WriteAysnc(IList<ArraySegment<byte>> buffers)
		{
			if(buffers==null) throw new ArgumentNullException("buffers");

			var completionSource=new TaskCompletionSource<WriteResults>();

			bool success=HandleNetworkCall(()=>
			{
				this.Socket.BeginSend(buffers,SocketFlags.None,EndWrite,completionSource);
			});

			if(success==false)
			{
				completionSource.SetException(new IOException("WriteAysnc failed"));
			}

			return completionSource.Task;
		}

		private void EndWrite(IAsyncResult result)
		{
			bool success=HandleNetworkCall(result,(ar)=>
			{
				var completionSource=(TaskCompletionSource<WriteResults>)ar.AsyncState;

				int bytesWritten=this.Socket.EndSend(ar);
				completionSource.SetResult(new WriteResults(this,bytesWritten));
				success=true;
			});

			if(success==false)
			{
				var completionSource=(TaskCompletionSource<WriteResults>)result.AsyncState;
				completionSource.SetException(new IOException("WriteAysnc failed"));
			}
		}	

		/// <summary>
		/// Closes the socket processor
		/// </summary>
		public virtual void Close()
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

				/*
				 * The socket class is thread safe, and Dispose may be called multiple times,
				 * so it's fine for us to call this multiple times from multiple threads
				 * without having to worry (phew!)
				 */
				MethodCall.AllowFail(()=>m_Socket.Dispose());
			}
		}

		/// <summary>
		/// Disposes of the processor
		/// </summary>
		public void Dispose()
		{
			Close();
		}

		/// <summary>
		/// Renders the socket processor as a string
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("ID={0}",m_ID);
		}
	}
}
