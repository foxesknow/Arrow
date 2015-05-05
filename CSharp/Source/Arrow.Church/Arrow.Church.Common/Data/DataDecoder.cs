using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Common.Data
{
	public class DataDecoder : IDisposable
	{
		private const int NullLength=-1;
		private readonly BinaryReader m_Reader;

		public DataDecoder(Stream stream)
		{
			if(stream==null) throw new ArgumentNullException("stream");

			m_Reader=new BinaryReader(stream,Encoding.UTF8,true);
		}

		public byte ReadByte()
		{
			return m_Reader.ReadByte();
		}

		public short ReadInt16()
		{
			return m_Reader.ReadInt16();
		}

		public int ReadInt32()
		{
			return m_Reader.ReadInt32();
		}

		public long ReadInt64()
		{
			return m_Reader.ReadInt64();
		}

		public bool ReadBoolean()
		{
			return m_Reader.ReadBoolean();
		}

		public string ReadString()
		{
			string value=null;

			int length=m_Reader.ReadInt32();
			if(length!=NullLength)
			{
				byte[] stringData=m_Reader.ReadBytes(length);
				value=Encoding.UTF8.GetString(stringData);
			}

			return value;
		}

		public T ReadEncodedData<T>(Func<DataDecoder,T> factory) where T:IEncodeData
		{
			bool hasValue=m_Reader.ReadBoolean();
			
			if(hasValue)
			{
				T value=factory(this);
				return value;
			}
			else
			{
				return default(T);
			}
		}

		public T ReadEncodedDataNeverNull<T>(Func<DataDecoder,T> factory) where T:IEncodeData
		{
			T value=factory(this);
			return value;
		}

		public void Dispose()
		{
			m_Reader.Dispose();
		}
	}
}
