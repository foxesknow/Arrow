using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Arrow.Collections;
using Arrow.Configuration;
using Arrow.Xml.ObjectCreation;
using Arrow.Xml;
using System.Diagnostics.CodeAnalysis;

namespace Arrow.Storage
{
	/// <summary>
	/// Provides access to names resources, such as files, webpages etc etc.
	/// The Arrow framework uses this to load data required at runtime.
	/// </summary>
	public static partial class StorageManager
	{
		private static Dictionary<string,Func<Uri,Accessor>> s_Accessors=new Dictionary<string,Func<Uri,Accessor>>(StringComparer.OrdinalIgnoreCase);

		private static readonly object s_Lock=new object();

		static StorageManager()
		{
			s_Accessors["file"]=uri=>new FileAccessor(uri);
			s_Accessors["http"]=uri=>new HttpAccessor(uri);
			s_Accessors[ResourceAccessor.Scheme]=uri=>new ResourceAccessor(uri);

			LoadFromAppConfig();
		}

		/// <summary>
		/// Adds an accessor to the manager
		/// </summary>
		/// <param name="name">The name to register the accessor under</param>
		/// <param name="accessorFactory">The factory to register</param>
		/// <exception cref="System.ArgumentNullException">name is null</exception>
		/// <exception cref="System.ArgumentNullException">accessorFactory is null</exception>
		public static void Add(string name, Func<Uri,Accessor> accessorFactory)
		{
			if(name==null) throw new ArgumentNullException("name");
			if(accessorFactory==null) throw new ArgumentNullException("accessorFactory");
			
			lock(s_Lock)
			{
				if(s_Accessors.ContainsKey(name)) throw new InvalidOperationException("accessor already registered: "+name);
				s_Accessors.Add(name,accessorFactory);
			}
		}

		/// <summary>
		/// Returns the data accessor for the scheme specified in a uri
		/// </summary>
		/// <param name="uri">A uri</param>
		/// <returns>The data accessor for the uri</returns>
		/// <exception cref="System.ArgumentNullException">name is null</exception>
		public static Accessor Get(Uri uri)
		{
			if(uri==null) throw new ArgumentNullException("uri");
			
			if(TryGetAcccessor(uri,out var accessor))
			{
				return accessor;
			}
			else
			{
				throw new IOException("accessor not found: "+uri.Scheme);
			}			
		}

		/// <summary>
		/// Attempts to get the data access for a uri
		/// </summary>
		/// <param name="uri">The uri whose accesor we're after</param>
		/// <param name="accessor">On success the accessor, or null if no accessor is found</param>
		/// <returns>true if the accessor is found, otherwise false</returns>
		/// <exception cref="System.ArgumentNullException">uri is null</exception>
		public static bool TryGetAcccessor(Uri uri, [NotNullWhen(true)] out Accessor? accessor)
		{
			if(uri==null) throw new ArgumentNullException("uri");

			Func<Uri,Accessor>? factory=null;

			lock(s_Lock)
			{
				
				if(s_Accessors.TryGetValue(uri.Scheme,out factory)==false)
				{
					accessor=null;
					return false;
				}
			}

			accessor=factory(uri);
			return true;
		}

		/// <summary>
		/// Determines if an accessor has been registered
		/// </summary>
		/// <param name="name">The accessor to check for</param>
		/// <returns>true is the accessor exist, otherwise false</returns>
		/// <exception cref="System.ArgumentNullException">name is null</exception>
		public static bool Contains(string name)
		{
			if(name==null) throw new ArgumentNullException("name");
		
			lock(s_Lock)
			{
				return s_Accessors.ContainsKey(name);
			}
		}

		/// <summary>
		/// Loads additional accessors instances from the application configuration.
		/// </summary>
		public static void LoadFromAppConfig()
		{
			var node=AppConfig.GetSectionXml(ArrowSystem.Name,"Arrow.Storage/StorageManager");
			if(node==null) return;
			
			List<DataAccessInfo> accessors=XmlCreation.CreateList<DataAccessInfo>(node.SelectNodesOrEmpty("*"));
			
			lock(s_Lock)
			{
				foreach(var info in accessors)
				{
					if(s_Accessors.ContainsKey(info.Name!)==false)
					{
						s_Accessors[info.Name!]=info.Factory!.Create;
					}
				}
			}
		}

		class DataAccessInfo
		{
			public string? Name{get; set;}
			public IAccessorFactory? Factory{get; set;}
		}
		
	}
}
