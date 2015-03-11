using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Net
{
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
		/// <param name="buffer">The bytes that make up the body</param>
		/// <returns>A body</returns>
		TBody CreateBody(THeader header, byte[] buffer);
		
		/// <summary>
		/// Determines the number of bytes for the body that will follow the header
		/// </summary>
		/// <param name="header">The header</param>
		/// <returns>The number of bytes for the body</returns>
		int GetBodySize(THeader header);
	}
}
