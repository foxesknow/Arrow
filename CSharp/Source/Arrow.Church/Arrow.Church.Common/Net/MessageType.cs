using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Common.Net
{
	/// <summary>
	/// The various message types used by the system.
	/// 
	/// A type that initiates a message has its low bit set to zero.
	/// A response to this message has its low bit set to 1
	/// </summary>
	public static class MessageType
	{
		public const int Request=2;			// 0b00000010
		public const int Response=3;		// 0b00000011
		public const int Ping=4;			// 0b00000100
		public const int Pong=5;			// 0b00000101
	}
}
