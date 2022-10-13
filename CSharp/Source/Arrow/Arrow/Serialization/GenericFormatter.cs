using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.IO;

#pragma warning disable SYSLIB0011

namespace Arrow.Serialization
{
	/// <summary>
	/// Provides an implementation of the IGenericFormatter interface
	/// </summary>
	/// <typeparam name="F">The type of formatter to use</typeparam>
	public class GenericFormatter<F> : IGenericFormatter where F:IFormatter, new()
	{
		private IFormatter m_Formatter=new F();
	
		/// <summary>
		/// Serializes a graph to a stream
		/// </summary>
		/// <typeparam name="T">The type of the object to serialize</typeparam>
		/// <param name="stream">The stream to write to</param>
		/// <param name="graph">The object to serialize</param>
		/// <exception cref="System.ArgumentNullException">stream is null</exception>
		public void Serialize<T>(Stream stream, T graph)
		{
			if(stream==null) throw new ArgumentNullException("stream");
			m_Formatter.Serialize(stream,graph);
		}

		/// <summary>
		/// Deserializes a graph from a stream
		/// </summary>
		/// <typeparam name="T">The type of the object to deserialize</typeparam>
		/// <param name="stream">The stream to read from</param>
		/// <returns>A deserialized object</returns>
		/// <exception cref="System.ArgumentNullException">stream is null</exception>
		public T Deserialize<T>(Stream stream)
		{
			if(stream==null) throw new ArgumentNullException("stream");
			return (T)m_Formatter.Deserialize(stream);
		}
		
		/// <summary>
		/// Writes an object graph to a file
		/// </summary>
		/// <typeparam name="T">The type of object to write</typeparam>
		/// <param name="filename">The file to write to</param>
		/// <param name="graph">The object to write</param>
		/// <param name="formatter">The formatter to use</param>
		/// <exception cref="System.ArgumentNullException">filename is null</exception>
		protected static void DoToFile<T>(string filename, T graph, IGenericFormatter formatter)
		{
			if(filename==null) throw new ArgumentNullException("filename");
			
			using(Stream stream=File.Create(filename))
			{
				formatter.Serialize<T>(stream,graph);
			}
		}
		
		/// <summary>
		/// Reads an object graph from a file
		/// </summary>
		/// <typeparam name="T">The type of object to read</typeparam>
		/// <param name="filename">The file to read from</param>
		/// <param name="formatter">The formatter to use</param>
		/// <returns>An object</returns>
		/// <exception cref="System.ArgumentNullException">filename is null</exception>
		protected static T DoFromFile<T>(string filename, IGenericFormatter formatter)
		{
			if(filename==null) throw new ArgumentNullException("filename");
			
			using(Stream stream=File.OpenRead(filename))
			{
				return formatter.Deserialize<T>(stream);
			}
		}
	}
}
