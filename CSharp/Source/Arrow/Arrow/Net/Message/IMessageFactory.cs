using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Memory.Pools;

namespace Arrow.Net.Message
{
	/// <summary>
	/// A message factory can create headers and body from binary data
	/// </summary>
	/// <typeparam name="THeader"></typeparam>
	/// <typeparam name="TBody"></typeparam>
	public interface IMessageFactory<THeader,TBody>
	{
		/// <summary>
		/// Returns the size of the header
		/// </summary>
		int HeaderSize{get;}

		/// <summary>
		/// Creates the header
		/// </summary>
		/// <param name="buffer">The bytes that make up the header</param>
		/// <returns>A header</returns>
		THeader CreateHeader(byte[] buffer);
		
		/// <summary>
		/// Creates the body
		/// </summary>
		/// <param name="header">The header that belongs with the message</param>
		/// <param name="buffer">The bytes that make up the body. The buffer may be larger that the value returned by GetBodySize due to buffer caching</param>
		/// <param name="bodySize">The size of the body, in bytes</param>
		/// <returns>A body</returns>
		TBody CreateBody(THeader header, byte[] buffer, int bodySize);
		
		/// <summary>
		/// Determines the number of bytes for the body that will follow the header
		/// </summary>
		/// <param name="header">The header</param>
		/// <returns>The number of bytes for the body</returns>
		int GetBodySize(THeader header);

		/// <summary>
		/// Returns a memory pool that will be used to allocate a buffer for the body
		/// </summary>
		/// <returns>A memory pool</returns>
		MemoryPool GetBodyPool();
	}
}
