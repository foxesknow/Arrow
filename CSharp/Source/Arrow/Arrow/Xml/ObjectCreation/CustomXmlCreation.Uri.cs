using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

using Arrow.Storage;
using Arrow.Net;

namespace Arrow.Xml.ObjectCreation
{
	public partial class CustomXmlCreation
	{
		private static readonly string SelectArg="select";
	
		/// <summary>
		/// Creates an object from the xml as the specified url
		/// </summary>
		/// <param name="uri">The uri to xml. The uri may contain a select query argument speciying an xpath</param>
		/// <returns>The object at the uri</returns>
		/// <exception cref="System.ArgumentNullException">uri is null</exception>
		public object UriCreate(Uri uri)
		{
			return UriCreate<object>(uri);
		}
		
		/// <summary>
		/// Creates an object from the xml as the specified url
		/// </summary>
		/// <typeparam name="T">The minimum type required of the object</typeparam>
		/// <param name="uri">The uri to xml. The uri may contain a select query argument speciying an xpath</param>
		/// <returns>The object at the uri</returns>
		/// <exception cref="System.ArgumentNullException">uri is null</exception>
		public T UriCreate<T>(Uri uri)
		{
			if(uri==null) throw new ArgumentNullException("uri");
			
			// See if the user want to select a node
			string? select=null;
			ProcessUri(ref uri,out select);
			
			using(Stream stream=StorageManager.Get(uri).OpenRead())
			{
				XmlDocument doc=new XmlDocument();
				doc.Load(stream);
				
				XmlNode? objectNode=doc.DocumentElement!;
				if(select!=null)
				{
					objectNode=doc.SelectSingleNode(select);
					if(objectNode==null) throw new XmlCreationException("select does not select any nodes");
				}
				
				return Create<T>(objectNode,uri);
			}
		}
		
		/// <summary>
		/// Creates an list from the xml as the specified url
		/// </summary>
		/// <typeparam name="T">The minimum type required of the objects in the list</typeparam>
		/// <param name="uri">The uri to xml. The uri may contain a select query argument speciying an xpath</param>
		/// <returns>A list containing the created objects</returns>
		/// <exception cref="System.ArgumentNullException">uri is null</exception>
		public List<T> UriCreateList<T>(Uri uri)
		{
			if(uri==null) throw new ArgumentNullException("uri");
			
			// See if the user want to select a node
			string? select=null;
			ProcessUri(ref uri,out select);
			
			using(Stream stream=StorageManager.Get(uri).OpenRead())
			{
				XmlDocument doc=new XmlDocument();
				doc.Load(stream);
				
				XmlNodeList? nodes=null;
				if(select==null)
				{
					nodes=doc.DocumentElement!.SelectNodes("*");
				}
				else
				{
					nodes=doc.SelectNodes(select);
					if(nodes==null) throw new XmlCreationException("select does not select any nodes: "+select);
				}
				
				return CreateList<T>(nodes!,uri);
			}
		}
		
		/// <summary>
		/// Populates a dictionary (by calls to Add) with data from the specified url.
		/// The name of the element/attribute is used as they key
		/// </summary>
		/// <typeparam name="T">The minimum type required of the objects in the dictionary</typeparam>
		/// <param name="dictionary">The dictionary to populate</param>
		/// <param name="uri">The uri to xml. The uri may contain a select query argument speciying an xpath</param>
		/// <exception cref="System.ArgumentNullException">dictionary is null</exception>
		/// <exception cref="System.ArgumentNullException">uri is null</exception>
		public void UriPopulateDictionary<T>(IDictionary<string,T> dictionary, Uri uri)
		{
			if(dictionary==null) throw new ArgumentNullException("dictionary");
			if(uri==null) throw new ArgumentNullException("uri");
			
			// See if the user want to select a node
			string? select=null;
			ProcessUri(ref uri,out select);
			
			using(Stream stream=StorageManager.Get(uri).OpenRead())
			{
				XmlDocument doc=new XmlDocument();
				doc.Load(stream);
				
				XmlNodeList? nodes=null;
				if(select==null)
				{
					nodes=doc.DocumentElement!.SelectNodes("*");
				}
				else
				{
					nodes=doc.SelectNodes(select);
					if(nodes==null) throw new XmlCreationException("select does not select any nodes: "+select);
				}
				
				PopulateDictionary(dictionary,nodes!,uri);
			}
		}
		
		/// <summary>
		/// Strips of the query and extracts the select query
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="select"></param>
		private void ProcessUri(ref Uri uri, out string? select)
		{
			// See if the user want to select a node
			select=null;			
			if(string.IsNullOrEmpty(uri.Query)==false)
			{
				var query=uri.ParseQuery();
				if(query.ContainsKey(SelectArg)) select=query[SelectArg];
			}
			
			uri=uri.StripQuery();
		}
	}
}
