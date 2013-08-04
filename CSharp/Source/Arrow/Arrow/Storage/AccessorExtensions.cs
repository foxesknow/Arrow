using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Arrow.Storage
{
	/// <summary>
	/// Useful extension methods for Accessor instances
	/// </summary>
	public static class AccessorExtensions
	{
		/// <summary>
		/// Attempts to load an xml document at a given location
		/// </summary>
		/// <param name="accessor">The accessor to read from</param>
		/// <returns>The document at the specified location</returns>
		/// <exception cref="System.ArgumentNullException">uri is null</exception>
		/// <exception cref="System.IO.IOException">The resource could not be opened</exception>
		public static XmlDocument ReadXmlDocument(this Accessor accessor)
		{
			XmlDocument doc=new XmlDocument();
			using(var stream=accessor.OpenRead())
			{
				doc.Load(stream);
			}
			
			return doc;
		}

		/// <summary>
		/// Attempts to load an xml document at a given location
		/// </summary>
		/// <param name="accessor">The accessor to read from</param>
		/// <returns>The document at the specified location</returns>
		/// <exception cref="System.ArgumentNullException">uri is null</exception>
		/// <exception cref="System.IO.IOException">The resource could not be opened</exception>
		public static XDocument ReadXDocument(this Accessor accessor)
		{
			using(var stream=accessor.OpenRead())
			using(StreamReader reader=new StreamReader(stream))
			{
				return XDocument.Load(reader);
			}
		}
		
		/// <summary>
		/// Attempts to read a string from a given location
		/// </summary>
		/// <param name="accessor">The accessor to read from</param>
		/// <returns>The string at the specified location</returns>
		/// <exception cref="System.ArgumentNullException">uri is null</exception>
		/// <exception cref="System.IO.IOException">The resource could not be opened</exception>
		public static string ReadString(this Accessor accessor)
		{
			using(var stream=accessor.OpenRead())
			using(StreamReader reader=new StreamReader(stream))
			{
				return reader.ReadToEnd();
			}
		}
	}
}
