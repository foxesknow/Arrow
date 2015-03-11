using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Arrow.Net
{
	public abstract class SocketProcessor
	{
		private long m_KeepReading=1;

		public bool KeepReading
		{
			get{return Interlocked.Read(ref m_KeepReading)==1;}
			set
			{
				Interlocked.Exchange(ref m_KeepReading,(value ? 1 : 0));
			}
		}

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
		/// Called when a network disconnect is detected
		/// </summary>
		protected abstract void OnDisconnected();

		/// <summary>
		/// Closes the socket processor
		/// </summary>
		public abstract void Close();

		/// <summary>
		/// Writes data to the socket
		/// </summary>
		/// <param name="buffer">The data to write</param>
		/// <param name="offset">The start of the data within the buffer</param>
		/// <param name="size">How much data to write</param>
		/// <returns>A task that will be signalled when the write completes</returns>
		public abstract Task<SocketProcessor> Write(byte[] buffer, int offset, int size);

	}
}
