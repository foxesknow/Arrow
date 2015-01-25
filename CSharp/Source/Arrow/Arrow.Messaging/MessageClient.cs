using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Messaging
{
	/// <summary>
	/// Base class for all message sender/receivers
	/// </summary>
	public abstract class MessageClient : IDisposable
	{
		/// <summary>
		/// Connects to a resource that messages can be sent to
		/// </summary>
		/// <param name="uri">The uri of the resource</param>
		public abstract void Connect(Uri uri);
		
		/// <summary>
		/// Disconnects from the messaging resource
		/// </summary>
		public abstract void Disconnect();
		
		/// <summary>
		/// Indicates if we are connected
		/// </summary>
		public abstract bool IsConnected{get;}
		
		/// <summary>
		/// Tidies up any resources
		/// </summary>
		public void Dispose()
		{
 			Disconnect();
		}
	}
}
