using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Arrow.Serialization
{
	/// <summary>
	/// Defines a typesafe way to serialize types
	/// </summary>
	public interface IGenericFormatter
	{
		/// <summary>
		/// Serializes a graph to a stream
		/// </summary>
		/// <typeparam name="T">The type of the object to serialize</typeparam>
		/// <param name="stream">The stream to write to</param>
		/// <param name="graph">The object to serialize</param>
		/// <exception cref="System.ArgumentNullException">stream is null</exception>
		void Serialize<T>(Stream stream, T graph);
		
		/// <summary>
		/// Deserializes a graph from a stream
		/// </summary>
		/// <typeparam name="T">The type of the object to deserialize</typeparam>
		/// <param name="stream">The stream to read from</param>
		/// <returns>A deserialized object</returns>
		/// <exception cref="System.ArgumentNullException">stream is null</exception>
		T Deserialize<T>(Stream stream);
	}
}
