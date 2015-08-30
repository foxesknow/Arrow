using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Arrow.IO
{
	/// <summary>
	/// Allows part of an array to be used as a stream
	/// </summary>
	public class ByteSegmentStream : Stream
	{
		private readonly ArraySegment<byte> m_Segment;
		private long m_Position=0;

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="data">The data to stream</param>
		public ByteSegmentStream(byte[] data) : this(new ArraySegment<byte>(data))
		{
		}

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="data">The data to stream</param>
		/// <param name="offset">The offset in the data to start from</param>
		/// <param name="count">The number of bytes in the array to use</param>
		public ByteSegmentStream(byte[] data, int offset, int count) : this(new ArraySegment<byte>(data,offset,count))
		{
		}

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="segment">The array segment to use</param>
		public ByteSegmentStream(ArraySegment<byte> segment)
		{
			m_Segment=segment;
		}

		/// <summary>
		/// How much space is left in the stream based on its current position
		/// </summary>
		public int AvailableSpace
		{
			get{return (int)(m_Segment.Count-m_Position);}
		}

		/// <summary>
		/// Returns true
		/// </summary>
		public override bool CanRead
		{
			get{return true;}
		}

		/// <summary>
		/// Returns true
		/// </summary>
		public override bool CanSeek
		{
			get{return true;}
		}

		/// <summary>
		/// Returns true
		/// </summary>
		public override bool CanWrite
		{
			get{return true;}
		}

		/// <summary>
		/// Does nothing
		/// </summary>
		public override void Flush()
		{
		}

		/// <summary>
		/// Returns the total number of bytes
		/// </summary>
		public override long Length
		{
			get{return m_Segment.Count;}
		}

		/// <summary>
		/// The current position
		/// </summary>
		public override long Position
		{
			get{return m_Position;}
			set
			{
				Seek(value,SeekOrigin.Begin);
			}
		}

		/// <summary>
		/// Reads data from the underlying array
		/// </summary>
		/// <param name="buffer">The buffer to populate</param>
		/// <param name="offset">Where to start in the buffer</param>
		/// <param name="count">The number of bytes to read</param>
		/// <returns>The number of bytes read</returns>
		public override int Read(byte[] buffer, int offset, int count)
		{
			if(buffer==null) throw new ArgumentNullException("buffer");
			if(offset<0) throw new ArgumentOutOfRangeException("offset");
			if(count<0) throw new ArgumentOutOfRangeException("count");
			if(offset+count>buffer.Length) throw new ArgumentException("read beyond buffer");

			int available=Math.Max(0,m_Segment.Count-(int)m_Position);
			int bytesToRead=Math.Min(count,available);

			var underlying=m_Segment.Array;
			int underlyingOffset=m_Segment.Offset;

			for(int i=0; i<bytesToRead; i++)
			{
				buffer[offset+i]=underlying[underlyingOffset+m_Position];
				m_Position++;
			}

			return bytesToRead;
		}

		/// <summary>
		/// Seeks to a new position in the array
		/// </summary>
		/// <param name="offset">The offset to seek to</param>
		/// <param name="origin">The origin to apply the offset to</param>
		/// <returns>The new position in the stream</returns>
		public override long Seek(long offset, SeekOrigin origin)
		{
			long newPosition=0;

			switch(origin)
			{
				case SeekOrigin.Begin:
					newPosition=offset;
					break;

				case SeekOrigin.Current:
					newPosition=m_Position+offset;
					break;

				case SeekOrigin.End:
					newPosition=m_Segment.Count+offset;
					break;

				default:
					throw new IOException("unsupported origin: "+origin.ToString());
			}

			//if(newPosition<0 || newPosition>=m_Segment.Count) throw new IOException("cannot seek beyond segment");

			m_Position=newPosition;
			return m_Position;
		}

		/// <summary>
		/// Not implemented
		/// </summary>
		/// <param name="value"></param>
		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Writes data to the underlying array
		/// </summary>
		/// <param name="buffer">The buffer to take the data from</param>
		/// <param name="offset">Where to start in the buffer</param>
		/// <param name="count">The number of bytes to write</param>
		public override void Write(byte[] buffer, int offset, int count)
		{
			if(buffer==null) throw new ArgumentNullException("buffer");
			if(offset<0) throw new ArgumentOutOfRangeException("offset");
			if(count<0) throw new ArgumentOutOfRangeException("count");
			if(offset+count>buffer.Length) throw new ArgumentException("write beyond buffer");

			if(m_Position+count>m_Segment.Count) throw new IOException("not enough space");

			var underlying=m_Segment.Array;
			int underlyingOffset=m_Segment.Offset;

			for(int i=0; i<count; i++)
			{
				underlying[underlyingOffset+m_Position]=buffer[offset+i];
				m_Position++;
			}
		}
	}
}
