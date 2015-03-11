using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Net
{
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
