using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Net.Message
{
	/// <summary>
	/// Specifies what a socket processor should do after letting 
	/// a message processor handle an new message
	/// </summary>
	public enum ReadMode
	{
		/// <summary>
		/// Keep reading messages from the socket
		/// </summary>
		KeepReading,
		
		/// <summary>
		/// Stop reading messages, but don't close the socket
		/// </summary>
		StopReading
	}
}
