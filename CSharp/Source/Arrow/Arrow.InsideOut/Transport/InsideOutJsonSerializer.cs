using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.IO;
using System.Text.Json.Serialization;

namespace Arrow.InsideOut.Transport;

/// <summary>
/// Converts object to and from json.
/// NOTE: Enums are serialized with their textual name, not as a number.
/// For a Flags enum then values are serializes as a csv list
/// </summary>
public sealed class InsideOutJsonSerializer
{
    private const int BufferSize = 1024;
    private const bool LeaveStreamOpen = true;
    private const bool DetectEncoding = true;

#if DEBUG
    private const bool Indent = true;
#else
    private const bool Indent = false;
#endif

    private readonly JsonSerializerOptions m_SerializeOptions = new()
    {
        WriteIndented = Indent,
        Converters =
        {
            new JsonStringEnumConverter(),
        }
    };

    /// <summary>
    /// Serializes to json
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="object"></param>
    /// <returns></returns>
    public string Serialize<T>(T @object) where T : class
    {
        return JsonSerializer.Serialize(@object, m_SerializeOptions);
    }

    /// <summary>
    /// Serializes to a stream
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="object"></param>
    /// <param name="stream"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void Serialize<T>(T @object, Stream stream) where T : class
    {
        if(stream is null) throw new ArgumentNullException(nameof(stream));

        JsonSerializer.Serialize(stream, @object, m_SerializeOptions);
    }

    /// <summary>
    /// Deserializes json from a string
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public T? Deserialize<T>(string json) where T : class
    {
        if(json is null) throw new ArgumentNullException(nameof(json));

        return JsonSerializer.Deserialize<T>(json, m_SerializeOptions);
    }

    /// <summary>
    /// Deserializes json from a stream
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <returns></returns>
    public T? Deserialize<T>(Stream stream) where T : class
    {
        return JsonSerializer.Deserialize<T>(stream, m_SerializeOptions);
    }
}
