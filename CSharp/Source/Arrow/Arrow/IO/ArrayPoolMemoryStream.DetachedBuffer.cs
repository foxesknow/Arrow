using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.IO
{
    public sealed partial class ArrayPoolMemoryStream
    {
        /// <summary>
        /// Manages a buffer detached from an ArrayPoolMemoryStream.
        /// Once you have detached the buffer you become the owner of the memory.
        /// You MUST call Dispose EXACTLY once to return the memory to it's unerlying pool.
        /// </summary>
        public readonly struct DetachedBuffer : IDisposable
        {
            /// <summary>
            /// Initializes the instance
            /// </summary>
            /// <param name="buffer"></param>
            /// <param name="length"></param>
            public DetachedBuffer(byte[] buffer, int length)
            {
                if(length < 0) throw new ArgumentException("length must be at least zero", nameof(length));

                this.Buffer = buffer;
                this.Length = length;
            }

            /// <summary>
            /// Returns any underlying buffer back to its pool.
            /// After calling this method any references to the 
            /// underlying buffer are invalid
            /// </summary>
            public void Dispose()
            {
                ArrayPoolMemoryStream.ReturnBuffer(this.Buffer);
            }

            /// <summary>
            /// Checks to see if an actual buffer exists
            /// </summary>
            [MemberNotNullWhen(true, nameof(Buffer))]
            public bool HasBuffer
            {
                get{return this.Buffer  is not null;}
            }
            
            /// <summary>
            /// Returns the underlying buffer, if available
            /// </summary>
            public byte[]? Buffer{get;}

            /// <summary>
            /// The length of the buffer that it is valid to use.
            /// NOTE: This may be different from the actual length of the buffer
            /// </summary>
            public int Length{get;}

            /// <summary>
            /// Returns the underlying memory as a Memory.
            /// NOTE: Once you have called Dispose() on this instance
            /// any memory the returned instance refers to is invalid
            /// </summary>
            /// <returns></returns>
            public Memory<byte> AsMemory()
            {
                if(this.Buffer is not null)
                {
                    return new(this.Buffer, 0, this.Length);
                }

                return default;
            }

            /// <summary>
            /// Returns the underlying buffer as a span.
            /// NOTE: Once you have called Dispose() on this instance
            /// any memory the returned instance refers to is invalid
            /// </summary>
            /// <returns></returns>
            public Span<byte> AsSpan()
            {
                if(this.Buffer is not null)
                {
                    return new(this.Buffer, 0, this.Length);
                }

                return default;
            }
        }
    }
}
