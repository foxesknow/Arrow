using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

using Arrow.IO;
using Arrow.Execution;

namespace Arrow.InsideOut.Transport;

/// <summary>
/// Serializes an object to json and encodes it as a gzip buffer
/// </summary>
public sealed class InsideOutEncoder
{
    private const bool LeaveOpen = true;

    private static readonly InsideOutJsonSerializer s_Serializer = new();

    /// <summary>
    /// Encodes the object to an array pool
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="object"></param>
    /// <returns></returns>
    public ArrayPoolReturner<byte> EncodeToPool<T>(T @object) where T : class
    {
        using(var stream = new ArrayPoolMemoryStream())
        {
            Encode(@object, stream);
            return stream.Detach();
        }
    }

    /// <summary>
    /// Encodes the object to a stream
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="object"></param>
    /// <param name="stream"></param>
    public void Encode<T>(T @object, Stream stream) where T : class
    {
        using(var zip = new GZipStream(stream, CompressionMode.Compress, LeaveOpen))
        {
            s_Serializer.Serialize(@object, zip);
        }
    }

    /// <summary>
    /// Encodes the object to a byte array
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="object"></param>
    /// <returns></returns>
    public Memory<byte> EncodeToMemory<T>(T @object) where T : class
    {
        using(var stream = new MemoryStream(256))
        {
            Encode(@object, stream);
            var length = (int)stream.Position;
            var buffer = stream.GetBuffer();

            return new(buffer, 0, length);
        }
    }

    /// <summary>
    /// Decodes an object from a stream
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <returns></returns>
    public T? Decode<T>(Stream stream) where T : class
    {
        using(var zip = new GZipStream(stream, CompressionMode.Decompress, LeaveOpen))
        {
            return s_Serializer.Deserialize<T>(zip);
        }
    }

    /// <summary>
    /// Decodes an object from an existing buffer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public T? Decode<T>(byte[] buffer, int offset, int count) where T : class
    {
        using(var stream = new MemoryStream(buffer, offset, count, false))
        {
            return Decode<T>(stream);
        }
    }
}
