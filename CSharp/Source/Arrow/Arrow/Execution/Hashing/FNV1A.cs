using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Execution.Hashing
{
    /// <summary>
    /// An implementation of the Fowler Noll Vo hash function (https://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function)
    /// This hashing algorithm is fast and provides reasonbly good hash coverage.
    /// Types larger than a byte are converted to a little endian array and the contents of the array is hashed.
    /// 
    /// NOTE: This is a mutable struct. If you need to pass it to other methods then pass it by reference, not by value.
    /// </summary>
    public ref struct FNV1A
    {
        private const uint OffsetBasis = 0x811c9dc5U;
        private const uint Prime = 0x01000193U;

        private uint m_Hash;

        /// <summary>
        /// Prepares the struct for hashing
        /// </summary>
        public void Begin()
        {
            m_Hash = OffsetBasis;
        }

        /// <summary>
        /// Returns the current hash value
        /// </summary>
        public readonly int HashValue
        {
            get{return unchecked((int)m_Hash);}
        }

        /// <inheritdoc/>
        public readonly override int GetHashCode()
        {
            return unchecked((int)m_Hash);
        }

        /// <summary>
        /// Applies a byte to the hasher
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The current hash value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Apply(byte value)
        {
            unchecked
            {
                m_Hash ^= value;
                m_Hash *= Prime;

                return (int)m_Hash;
            }
        }

        /// <summary>
        /// Applies a short to the hasher
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The current hash value</returns>
        public int Apply(short value)
        {
            Span<byte> buffer = stackalloc byte[sizeof(short)];
            BinaryPrimitives.WriteInt16LittleEndian(buffer, value);
            return Apply(buffer);
        }

        /// <summary>
        /// Applies an int to the hasher
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The current hash value</returns>
        public int Apply(int value)
        {
            Span<byte> buffer = stackalloc byte[sizeof(int)];
            BinaryPrimitives.WriteInt32LittleEndian(buffer, value);
            return Apply(buffer);
        }

        /// <summary>
        /// Applies a long to the hasher
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The current hash value</returns>
        public int Apply(long value)
        {
            Span<byte> buffer = stackalloc byte[sizeof(long)];
            BinaryPrimitives.WriteInt64LittleEndian(buffer, value);
            return Apply(buffer);
        }

        /// <summary>
        /// Applies a float to the hasher
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The current hash value</returns>
        public int Apply(float value)
        {
            Span<byte> buffer = stackalloc byte[sizeof(float)];
            BinaryPrimitives.WriteSingleLittleEndian(buffer, value);
            return Apply(buffer);
        }

        /// <summary>
        /// Applies a double to the hasher
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The current hash value</returns>
        public int Apply(double value)
        {
            Span<byte> buffer = stackalloc byte[sizeof(double)];
            BinaryPrimitives.WriteDoubleLittleEndian(buffer, value);
            return Apply(buffer);
        }

        /// <summary>
        /// Applies a sequence of bytes to the hasher
        /// </summary>
        /// <param name="values"></param>
        /// <returns>The current hash value</returns>
        public int Apply(scoped ReadOnlySpan<byte> values)
        {
            unchecked
            {
                var length = values.Length;

                for(int i = 0; i < length; i++)
                {
                    var value = values[i];
                    m_Hash ^= value;
                    m_Hash *= Prime;
                }

                return (int)m_Hash;
            }
        } 

        /// <summary>
        /// Creates a FNV1a hasher that has had Begin() called on it
        /// </summary>
        /// <returns></returns>
        public static FNV1A Make()
        {
            var hasher = new FNV1A();
            hasher.Begin();

            return hasher;
        }
    }
}
