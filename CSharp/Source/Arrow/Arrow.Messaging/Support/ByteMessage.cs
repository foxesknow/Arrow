using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Messaging.Support
{
	/// <summary>
	/// Provides a reasonable default implementation of a byte message
	/// for use in specific implementations that do not have a concreate
	/// message class, such as 29West
	/// </summary>
	[Serializable]
	public class ByteMessage : MessageBase, IByteMessage
	{
		private readonly byte[] m_Data;
	
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="data">The byte data that makes up the message</param>
		public ByteMessage(byte[] data)
		{
			m_Data = data;
		}
	
		/// <summary>
		/// The bytes in the message
		/// </summary>
		public virtual byte[] Data
		{
			get{return m_Data;}
		}
	}
}
