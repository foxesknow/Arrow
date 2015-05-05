using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Common.Data
{
	public class DataEncoder : IDisposable
	{
		private const int NullLength=-1;
		private readonly BinaryWriter m_Writer;

		public DataEncoder(Stream stream)
		{
			if(stream==null) throw new ArgumentNullException("stream");

			m_Writer=new BinaryWriter(stream,Encoding.UTF8,true);
		}

		public void Write(byte value)
		{
			m_Writer.Write(value);
		}

		public void Write(short value)
		{
			m_Writer.Write(value);
		}

		public void Write(int value)
		{
			m_Writer.Write(value);
		}

		public void Write(long value)
		{
			m_Writer.Write(value);
		}

		public void Write(bool value)
		{
			m_Writer.Write(value);
		}

		public void Write(string value)
		{
			if(value==null)
			{
				m_Writer.Write(NullLength);
			}
			else
			{
				byte[] bytes=Encoding.UTF8.GetBytes(value);
				m_Writer.Write(bytes.Length);
				m_Writer.Write(bytes);
			}
		}

		public void Write(IEncodeData encodeData)
		{
			bool hasValue=(encodeData!=null);

			m_Writer.Write(hasValue);
			if(hasValue) encodeData.Encode(this);
		}

		public void WriteNeverNull(IEncodeData encodeData)
		{
			if(encodeData==null) throw new ArgumentNullException("encodeData");
			encodeData.Encode(this);
		}

		public void Dispose()
		{
			m_Writer.Dispose();
		}
	}
}
