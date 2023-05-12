using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

using Arrow.Execution;
using Arrow.Threading.Tasks;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace Arrow.InsideOut.Transport.Tcp
{
    internal static class StreamSupport
    {
        /// <summary>
        /// Writes a buffer to a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static async ValueTask Write(Stream stream, byte[] buffer, int offset, int count, CancellationToken ct)
        {
            await stream.WriteAsync(Versions.VersionAsBuffer, 0, Versions.VersionAsBuffer.Length, ct).ContinueOnAnyContext();
            await stream.WriteAsync(BitConverter.GetBytes(count), 0, sizeof(int), ct).ContinueOnAnyContext();
            await stream.WriteAsync(buffer, offset, count, ct).ContinueOnAnyContext();

            await stream.FlushAsync().ContinueOnAnyContext();
        }

        /// <summary>
        /// Reads a buffer from a stream into an array backed by a pool
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        /// <exception cref="IOException"></exception>
        /// <exception cref="InsideOutException"></exception>
        public static async ValueTask<ArrayPoolReturner<byte>> Read(Stream stream, CancellationToken ct)
        {
            const int headerSize = sizeof(int) + sizeof(int);
            var pool = ArrayPool<byte>.Shared;

            // We'll grab a buffer up front, any hopefully it'll be big enough
            var initialSize = Math.Max(256, headerSize);
            var buffer = pool.Rent(initialSize);

            var headerByteOffset = 0;
            var headersBytesToRead = headerSize;

            while(headersBytesToRead != 0)
            {
                var read = await stream.ReadAsync(buffer, headerByteOffset, headersBytesToRead, ct).ContinueOnAnyContext();
                if(read == 0) ReturnAndThrow(pool, buffer, "not enough data");

                headerByteOffset += read;
                headersBytesToRead -= read;
            }

            var version = BitConverter.ToInt32(buffer, 0);
            if(version != Versions.Version) ReturnAndThrow(pool, buffer, "incorrect version");

            var bufferLength = BitConverter.ToInt32(buffer, sizeof(int));
            
            // It the buffer isn't big enough we'll need to get a new one
            if(bufferLength > buffer.Length)
            {
                pool.Return(buffer);
                buffer = pool.Rent(bufferLength);
            }

            var bytesOffset = 0;
            var bytesToRead = bufferLength;

            while(bytesToRead != 0)
            {
                var read = await stream.ReadAsync(buffer, bytesOffset, bytesToRead, ct).ContinueOnAnyContext();
                if(read == 0) ReturnAndThrow(pool, buffer, "not enough data");

                bytesOffset += read;
                bytesToRead -= read;
            }

            return new(pool, buffer, 0, bufferLength);

            [DoesNotReturn]
            static void ReturnAndThrow(ArrayPool<byte> pool, byte[] buffer, string message)
            {
                pool.Return(buffer);
                throw new IOException(message);
            }
        }
    }
}
