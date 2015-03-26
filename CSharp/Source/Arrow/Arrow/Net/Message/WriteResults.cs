﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Net.Message
{
	/// <summary>
	/// Holds the outcome as an async write to a socket processor
	/// </summary>
	public struct WriteResults : IEquatable<WriteResults>
	{
		private readonly SocketProcessor m_SocketProcessor;
		private readonly int m_BytesWritten;

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="socketProcessor">The socket processor that was written to</param>
		/// <param name="bytesWritten">The number of bytes that were written</param>
		public WriteResults(SocketProcessor socketProcessor, int bytesWritten)
		{
			m_SocketProcessor=socketProcessor;
			m_BytesWritten=bytesWritten;
		}

		/// <summary>
		/// The socket processor that was written to
		/// </summary>
		public SocketProcessor SocketProcessor
		{
			get{return m_SocketProcessor;}
		}

		/// <summary>
		/// The number of bytes written
		/// </summary>
		public int BytesWritten
		{
			get{return m_BytesWritten;}
		}

		/// <summary>
		/// Compares two instances
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(WriteResults other)
		{
			return m_SocketProcessor.ID==other.m_SocketProcessor.ID &&m_BytesWritten==other.m_BytesWritten;
		}

		/// <summary>
		/// Compares two instances
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if(obj==null) return false;

			if(obj is WriteResults)
			{
				return Equals((WriteResults)obj);
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Generates a hash code
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return (m_SocketProcessor.ID.GetHashCode()*31)+m_BytesWritten;
		}

		/// <summary>
		/// Renders the struct as a string
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format(", BytesWritten={1}",m_SocketProcessor,m_BytesWritten);
		}
	}
}