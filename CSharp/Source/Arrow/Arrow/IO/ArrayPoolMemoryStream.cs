using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Buffers;
using System.Threading;

namespace Arrow.IO
{
    /// <summary>
    /// A write-only non seekable (forward only) stream that uses an ArrayPool to allocate
    /// its buffers and therefore reduce the load on the gargbage collector.
    /// </summary>
    public sealed partial class ArrayPoolMemoryStream : Stream
    {
        // We'll use our own pool rather than the shared one to minimize contention
        private static readonly ArrayPool<byte> s_BytePools = ArrayPool<byte>.Create();

        private byte[] m_Buffer;
        private int m_Position = 0;

        private readonly int m_InitialCapacity;

        /// <summary>
        /// Initializes the stream with a default buffer size
        /// </summary>
        public ArrayPoolMemoryStream() : this(64)
        {
        }

        /// <summary>
        /// Initializes the stream with a given size
        /// </summary>
        /// <param name="initialCapacity"></param>
        /// <exception cref="ArgumentException"></exception>
        public ArrayPoolMemoryStream(int initialCapacity)
        {
            if(initialCapacity < 0) throw new ArgumentException("initial capacity must be at least 0", nameof(initialCapacity));
            
            m_InitialCapacity = initialCapacity;
            m_Buffer = MakeInitialBuffer(initialCapacity);
        }

        /// <inheritdoc/>
        public override void Close()
        {
            base.Close();

            if(m_Buffer.Length != 0)
            {
                s_BytePools.Return(m_Buffer);
                m_Buffer = Array.Empty<byte>();
                m_Position = 0;
            }
        }

        /// <summary>
        /// Detached the pooled buffer from the stream.
        /// This resets the buffer back to its initial state.
        /// 
        /// NOTE: The caller is now responsible for the buffer.
        /// You must call DetachedBuffer.Dispose EXACTLY once to
        /// return the buffer to the pool
        /// </summary>
        /// <returns></returns>
        public DetachedBuffer Detach()
        {
            var detachedBuffer = new DetachedBuffer(m_Buffer, m_Position);

            m_Position = 0;
            m_Buffer = MakeInitialBuffer(m_InitialCapacity);

            return detachedBuffer;
        }

        /// <summary>
        /// Returns false
        /// </summary>
        public override bool CanRead
        {
            get{return false;}
        }

        /// <summary>
        /// Returns false
        /// </summary>
        public override bool CanSeek
        {
            get{return false;}
        }

        /// <summary>
        /// Returns true
        /// </summary>
        public override bool CanWrite
        {
            get{return true;}
        }

        /// <summary>
        /// Returns the length of the stream
        /// </summary>
        public override long Length
        {
            get{return m_Position;}
        }

        /// <summary>
        /// Returns the current position
        /// </summary>
        public override long Position
        {
            get{return m_Position;}
            set{throw new NotImplementedException();}
        }

        /// <inheritdoc/>
        public override void Flush()
        {
            // Does nothing
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void WriteByte(byte value)
        {
            EnsureCapacity(1);
            m_Buffer[m_Position] = value;
            
            m_Position++;
        }

        /// <inheritdoc/>
        public override void Write(ReadOnlySpan<byte> buffer)
        {
            if(buffer.Length == 0) return;

            EnsureCapacity(buffer.Length);
            var span = new Span<byte>(m_Buffer, m_Position, buffer.Length);
            buffer.CopyTo(span);

            m_Position += buffer.Length;
        }

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            ValidateBufferArguments(buffer, offset, count);
            if(count == 0) return;
            
            EnsureCapacity(count);
            Buffer.BlockCopy(buffer, 0, m_Buffer, m_Position, count);
            m_Position += count;
        }

        /// <inheritdoc/>
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {         
            if(cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled(cancellationToken);
            }

            Write(buffer, offset, count);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if(cancellationToken.IsCancellationRequested)
            {
                return ValueTask.FromCanceled(cancellationToken);
            }

            Write(buffer.Span);
            return default;
        }

        private void EnsureCapacity(int additionalCapacity)
        {
            var requiredBufferLength = m_Position + additionalCapacity;
            if(requiredBufferLength > m_Buffer.Length)
            {
                // We need to allocate a new buffer
                var newBufferLength = requiredBufferLength * 2;
                var newBuffer = s_BytePools.Rent(newBufferLength);
                Buffer.BlockCopy(m_Buffer, 0, newBuffer, 0, m_Position);

                s_BytePools.Return(m_Buffer);
                m_Buffer = newBuffer;
            }
        }

        private byte[] MakeInitialBuffer(int initialCapacity)
        {
            if(initialCapacity == 0)
            {
                return Array.Empty<byte>();
            }
            else
            {
                return s_BytePools.Rent(initialCapacity);
            }
        }

        private static void ReturnBuffer(byte[]? buffer)
        {
            if(buffer is not null && buffer.Length != 0)
            {
                s_BytePools.Return(buffer);
            }
        }
    }
}
