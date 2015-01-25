using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Messaging
{
	/// <summary>
	/// Base class for all byte stream messages
	/// </summary>
	public class ByteStreamMessageEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="data">The data that makes up the message</param>
		public ByteStreamMessageEventArgs(byte[] data)
		{
			this.Data=data;
		}
		
		/// <summary>
		/// The data for the message
		/// </summary>
		public byte[] Data{get;private set;}
	}
}
