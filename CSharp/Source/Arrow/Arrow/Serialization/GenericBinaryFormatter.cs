using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

#pragma warning disable SYSLIB0011

namespace Arrow.Serialization
{
	/// <summary>
	/// Serializes objects as binary
	/// </summary>
	public class GenericBinaryFormatter : GenericFormatter<BinaryFormatter>
	{
		/// <summary>
		/// Serializes an object to a byte array
		/// </summary>
		/// <param name="obj">The object to serialize</param>
		/// <returns>An arrary containing the serialized state</returns>
		public static byte[] ToArray(object obj)
		{
			using(MemoryStream stream=new MemoryStream())
			{
				new GenericBinaryFormatter().Serialize(stream,obj);
				return stream.ToArray();
			}
		}
		
		/// <summary>
		/// Serializes an object to a byte compressed array
		/// </summary>
		/// <param name="obj">The object to serialize</param>
		/// <returns>An arrary containing the serialized state</returns>
		public static byte[] ToCompressedArray(object obj)
		{
			using(MemoryStream stream=new MemoryStream())
			{
				using(DeflateStream compressedStream=new DeflateStream(stream,CompressionMode.Compress))
				{
					new GenericBinaryFormatter().Serialize(compressedStream,obj);
					compressedStream.Flush();
				}
				
				return stream.ToArray();
			}
		}
		
		/// <summary>
		/// Serializes an object to a stream
		/// </summary>
		/// <param name="stream">The stream to write to</param>
		/// <param name="obj">The object to serialize</param>
		public static void ToStream(Stream stream, object obj)
		{
			new GenericBinaryFormatter().Serialize(stream,obj);
		}
		
		/// <summary>
		/// Deserializes an object from an array
		/// </summary>
		/// <typeparam name="T">The type of the object in the array</typeparam>
		/// <param name="array">The array to deserialize from </param>
		/// <returns>An instance of T</returns>
		public static T FromArray<T>(byte[] array)
		{
			if(array==null) throw new ArgumentNullException("array");
			
			using(MemoryStream stream=new MemoryStream(array))
			{
				GenericBinaryFormatter formatter=new GenericBinaryFormatter();
				return formatter.Deserialize<T>(stream);
			}
		}
		
		/// <summary>
		/// Deserializes an object from a gzipped compressed array
		/// </summary>
		/// <typeparam name="T">The type of the object in the array</typeparam>
		/// <param name="array">The array to deserialize from </param>
		/// <returns>An instance of T</returns>
		public static T FromCompressedArray<T>(byte[] array)
		{
			if(array==null) throw new ArgumentNullException("array");
			
			using(MemoryStream stream=new MemoryStream(array))
			using(DeflateStream compressedStream=new DeflateStream(stream,CompressionMode.Decompress))
			{
				GenericBinaryFormatter formatter=new GenericBinaryFormatter();
				return formatter.Deserialize<T>(compressedStream);
			}
		}

		/// <summary>
		/// Deserializes an object form a stream
		/// </summary>
		/// <typeparam name="T">The type of object expected</typeparam>
		/// <param name="stream">The stream to deserialize from</param>
		/// <returns>An instance of T</returns>
		public static T FromStream<T>(Stream stream)
		{
			GenericBinaryFormatter formatter=new GenericBinaryFormatter();
			return formatter.Deserialize<T>(stream);
		}
	
		/// <summary>
		/// Writes an object graph to a file
		/// </summary>
		/// <typeparam name="T">The type of object to write</typeparam>
		/// <param name="filename">The file to write to</param>
		/// <param name="graph">The object to write</param>
		/// <exception cref="System.ArgumentNullException">filename is null</exception>
		public static void ToFile<T>(string filename, T graph)
		{
			DoToFile(filename,graph,new GenericBinaryFormatter());
		}
		
		/// <summary>
		/// Reads an object graph from a file
		/// </summary>
		/// <typeparam name="T">The type of object to read</typeparam>
		/// <param name="filename">The file to read from</param>
		/// <returns>An object</returns>
		/// <exception cref="System.ArgumentNullException">filename is null</exception>
		public static T FromFile<T>(string filename)
		{
			return DoFromFile<T>(filename,new GenericBinaryFormatter());
		}
	}
}

#pragma warning restore SYSLIB0011