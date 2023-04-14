using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace Arrow.IO
{
    /// <summary>
    /// A stream that reads from one stream and writes to another.
    ///
    /// This is useful for testing. For example it can be used to mimic a NetworkStream.
    /// </summary>
    public sealed class SplitStream : Stream
    {
        private readonly Stream m_Read;
        private readonly Stream m_Write;
        private readonly bool m_LeaveOpen;

        private bool m_Disposed;

        /// <summary>
        /// Initializes the instance.
        /// The streams will be left open when the instance is closed
        /// </summary>
        /// <param name="read"></param>
        /// <param name="write"></param>
        public SplitStream(Stream read, Stream write) : this(read, write, true)
        {
        }

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="read"></param>
        /// <param name="write"></param>
        /// <param name="leaveOpen"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public SplitStream(Stream read, Stream write, bool leaveOpen)
        {
            if(read is null) throw new ArgumentNullException(nameof(read));
            if(read.CanRead == false) throw new ArgumentNullException("read stream is not readable", nameof(read));

            if(write is null) throw new ArgumentNullException(nameof(write));
            if(read.CanWrite == false) throw new ArgumentNullException("write stream is not writable", nameof(write));

            m_Read = read;
            m_Write = write;
            m_LeaveOpen = leaveOpen;
        }

        /// <summary>
        /// The stream that reads will read from
        /// </summary>
        public Stream ReadStream
        {
            get{return m_Read;}
        }

        /// <summary>
        /// The stream that writes will write to
        /// </summary>
        public Stream WriteStream
        {
            get{return m_Write;}
        }

        /// <inheritdoc/>
        public override long Length
        {
            get{throw new NotImplementedException();}
        }

        /// <inheritdoc/>
        public override long Position
        {
            get{throw new NotImplementedException();}
            set{throw new NotImplementedException();}
        }

        /// <inheritdoc/>
        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool CanRead
        {
            get{return true;}
        }

        /// <inheritdoc/>
        public override bool CanWrite
        {
            get{return true;}
        }

        /// <inheritdoc/>
        public override bool CanSeek
        {
            get{return false;}
        }

        /// <inheritdoc/>
        public override bool CanTimeout
        {
            get{return m_Read.CanTimeout || m_Write.CanTimeout;}
        }

        /// <inheritdoc/>
        public override int ReadTimeout 
        { 
            get{return m_Read.ReadTimeout;}
            set{m_Read.ReadTimeout = value;}
        }

        /// <inheritdoc/>
        public override int WriteTimeout 
        { 
            get{return m_Write.WriteTimeout;}
            set{m_Write.WriteTimeout = value;}
        }

        /// <inheritdoc/>
        public override int ReadByte()
        {
            ThrowIfDisposed();
            return m_Read.ReadByte();
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            ThrowIfDisposed();
            return m_Read.Read(buffer, offset, count);
        }

        /// <inheritdoc/>
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            return m_Read.ReadAsync(buffer, offset, count, cancellationToken);
        }

        /// <inheritdoc/>
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            ThrowIfDisposed();
            return m_Read.BeginRead(buffer, offset, count, callback, state);
        }

        /// <inheritdoc/>
        public override int EndRead(IAsyncResult asyncResult)
        {
            ThrowIfDisposed();
            return m_Read.EndRead(asyncResult);
        }

        /// <inheritdoc/>
        public override void WriteByte(byte value)
        {
            ThrowIfDisposed();
            m_Write.WriteByte(value);
        }

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            ThrowIfDisposed();
            m_Write.Write(buffer, offset, count);
        }

        /// <inheritdoc/>
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            return m_Write.WriteAsync(buffer, offset, count, cancellationToken);
        }

        /// <inheritdoc/>
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            ThrowIfDisposed();
            return m_Write.BeginWrite(buffer, offset, count, callback, state);
        }

        /// <inheritdoc/>
        public override void EndWrite(IAsyncResult asyncResult)
        {
            ThrowIfDisposed();
            m_Write.EndWrite(asyncResult);
        }

        /// <inheritdoc/>
        public override void Flush()
        {
            ThrowIfDisposed();
            m_Write.Flush();
        }

        /// <inheritdoc/>
        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            return m_Write.FlushAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public override void Close()
        {
            if(m_Disposed == false)
            {
                if(m_LeaveOpen == false)
                {
                    m_Read.Close();
                    m_Write.Close();
                }

                m_Disposed = true;
            }

            base.Close();
        }

        private void ThrowIfDisposed()
        {
            if(m_Disposed) throw new ObjectDisposedException(nameof(SplitStream));
        }
    }
}
