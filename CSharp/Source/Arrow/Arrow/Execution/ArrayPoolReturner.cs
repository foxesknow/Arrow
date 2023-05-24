using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Execution
{
    /// <summary>
    /// Holds onto a buffer from an ArrayPool, returning it when disposed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct ArrayPoolReturner<T> : IDisposable
    {
        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        public ArrayPoolReturner(ArrayPool<T> pool, T[]? buffer, int start, int length)
        {
            ArgumentNullException.ThrowIfNull(pool);

            if(buffer is null)
            {
                if(start != 0) throw new ArgumentException("start must be zero for a null buffer", nameof(start));
                if(length != 0) throw new ArgumentException("length must be zero for a null buffer", nameof(length));
            }
            else
            {
                if(start < 0) throw new ArgumentOutOfRangeException(nameof(start));
                if(length < 0) throw new ArgumentOutOfRangeException(nameof(length));
                if(buffer.Length - start < length) throw new ArgumentException("invalid range for buffer");
            }

            this.Pool = pool;
            this.Buffer = buffer;
            this.Start = start;
            this.Length = length;
        }

        /// <summary>
        /// Returns the buffer to its pool.
        /// 
        /// NOTE: You must call this method EXACTLY once.
        /// </summary>
        public void Dispose()
        {
            if(this.Buffer is not null && this.Pool is not null)
            {
                this.Pool.Return(this.Buffer);
            }
        }

        /// <summary>
        /// Checks to see if an actual buffer exists
        /// </summary>
        [MemberNotNullWhen(true, nameof(Buffer))]
        public bool HasBuffer
        {
            get{return this.Buffer is not null;}
        }

        /// <summary>
        /// The pool to return to 
        /// </summary>
        private ArrayPool<T>? Pool{get;}
        
        /// <summary>
        /// The buffer that must be returned
        /// </summary>
        public T[]? Buffer{get;}

        /// <summary>
        /// The start offset within the buffer that is usable
        /// </summary>
        public int Start{get;}
        
        /// <summary>
        /// The logical size of the buffer
        /// </summary>
        public int Length{get;}

        /// <summary>
        /// Returns the underlying memory as a Memory.
        /// NOTE: Once you have called Dispose() on this instance
        /// any memory the returned instance refers to is invalid
        /// </summary>
        /// <returns></returns>
        public Memory<T> AsMemory()
        {
            if(this.Buffer is not null)
            {
                return new(this.Buffer, this.Start, this.Length);
            }

            return default;
        }

        /// <summary>
        /// Returns the underlying buffer as a span.
        /// NOTE: Once you have called Dispose() on this instance
        /// any memory the returned instance refers to is invalid
        /// </summary>
        /// <returns></returns>
        public Span<T> AsSpan()
        {
            if(this.Buffer is not null)
            {
                return new(this.Buffer, this.Start, this.Length);
            }

            return default;
        }
    }

    /// <summary>
    /// Useful factory methods
    /// </summary>
    public static class ArrayPoolReturner
    {
        /// <summary>
        /// Creates a returner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pool"></param>
        /// <param name="buffer"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static ArrayPoolReturner<T> ForBuffer<T>(ArrayPool<T> pool, T[]? buffer, int start, int length)
        {
            return new(pool, buffer, start, length);
        }
    }
}
