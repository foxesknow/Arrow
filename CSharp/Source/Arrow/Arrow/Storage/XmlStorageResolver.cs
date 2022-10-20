using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Net;

namespace Arrow.Storage
{
	/// <summary>
	/// Resolves enternal XML resources using the StorageManager.
	/// </summary>
	public class XmlStorageResolver : XmlResolver
	{
		private ICredentials m_Credentials = default!;

		/// <summary>
		/// Stores any credentials
		/// </summary>
		public override ICredentials Credentials
		{
			set{m_Credentials=value;}
		}

		/// <summary>
		/// Returns a stream to the specified uri
		/// </summary>
		/// <param name="absoluteUri">The path to the resource</param>
		/// <param name="role">Not used</param>
		/// <param name="ofObjectToReturn">The type to return. Must be System.IO.Stream</param>
		/// <returns>A Stream object to resource</returns>
		public override object GetEntity(Uri absoluteUri, string? role, Type? ofObjectToReturn)
		{
			if(ofObjectToReturn!=null && ofObjectToReturn!=typeof(Stream))
			{
				throw new XmlException("unsupported return type");
			}
			
			return StorageManager.Get(absoluteUri).OpenRead();
		}
	}
}
