using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using System.Configuration;
using System.Collections.Specialized;

using Arrow.Storage;
using Arrow.Xml;
using Arrow.Xml.ObjectCreation;
using Arrow.Text;

namespace Arrow.Configuration
{
	/// <summary>
	/// Provided access to application configuration.
	/// By default the config comes from the application config (X.exe.config).
	/// <remarks>
	/// To reduce the forest of configSections/section elements at the top of the file
	/// the Arrow config system splits the config file into systems. For each system you add
	/// one section and then place all the config within that element. For example, all
	/// Arrow configuration is held within a Arrow element. To configure this add:
	/// <![CDATA[
	/// <configSections>
	///		<section name="Arrow" type="Arrow.Configuration.ArrowSectionHandler,Arrow"/>
	///	</configSections>
	/// ]]>
	/// to the top off the app.config file.
	/// 
	/// If you store application configuration is a file other that the app.config then you will
	/// not require a configSections element in this file. However, you must have a root "configuration"
	/// element and place the various systems within the element.
	/// 
	/// Subsections within the system can pull in their data from external sources. For example:
	/// <![CDATA[
	/// <Arrow>
	///   <Arrow.Subsystem uri="location" />
	/// </Arrow>
	/// ]]>
	/// </remarks>
	/// </summary>
	public static class AppConfig
	{
		private static XmlDocument? s_ConfigDocument;
		private static IConfigFile s_CurrentConfigFile = default!;

		private static object s_AppSettingsSyncRoot = new object();
		private static NameValueCollection? s_AllAppSettings;
		private static Exception? s_AppSettingsException;

		private static Lazy<ConnectionStringSettingsCollection> s_LazyConnectionStringSettings;
		
		static AppConfig()
		{
			// To bootstrap we'll look for an app.config file
			try
			{
				s_CurrentConfigFile=new AppDomainConfigFile();
				s_ConfigDocument=s_CurrentConfigFile.LoadConfig();
			}
			catch
			{
				s_ConfigDocument=null;
			}

			s_LazyConnectionStringSettings = ResetDynamicState();
		}
		
		/// <summary>
		///  Forces any static initialization.
		///  This is useful if you want initialization to be deterministic
		/// </summary>
		public static void ExplicitInitialize()
		{
			// Does nothing. Calling this method will invoke the static constructor
		}
		
		/// <summary>
		/// Returns the document used for configuration, or null if no document exists
		/// </summary>
		internal static XmlDocument? ConfigDocument
		{
			get{return s_ConfigDocument;}
		}
		
		/// <summary>
		/// Replaces the application configuration with a new "file"
		/// </summary>
		/// <param name="configFile">The config file to use</param>
		/// <exception cref="System.ArgumentNullException">configFile is null</exception>
		public static void ReplaceConfig(IConfigFile configFile)
		{
			if(configFile is null) throw new ArgumentNullException(nameof(configFile));
			
			s_ConfigDocument = configFile.LoadConfig();
			s_CurrentConfigFile = configFile;
			s_LazyConnectionStringSettings = ResetDynamicState();
		}
		
		/// <summary>
		/// Returns the current config file loader in use
		/// </summary>
		public static IConfigFile ConfigFile
		{
			get{return s_CurrentConfigFile;}
		}

		/// <summary>
		/// Returns the connectionString collection from the app.config.
		/// NOTE: No variable expansion takes place on any of the values in the collection.
		/// </summary>
		public static ConnectionStringSettingsCollection ConnectionStrings
		{
			get{return s_LazyConnectionStringSettings.Value;}
		}

		/// <summary>
		/// Returns the appSettings collection.
		/// NOTE: No variable expansion takes place on the values in the collection.
		/// </summary>
		public static NameValueCollection AppSettings
		{
			get{return NewAppSettings();}
		}

		/// <summary>
		/// Reads an "old-style" NameValueSectionHandler style configuration section.
		/// This method is intended to ease the transition to the other methods in this class
		/// NOTE: The collection WILL have variable expansion applied to its values
		/// </summary>
		/// <returns></returns>
		public static NameValueCollection? LegacyGetConfig(string path)
		{
			return DoLegacyGetConfig(path, true);
		}

		private static NameValueCollection NewAppSettings()
		{
			lock(s_AppSettingsSyncRoot)
			{
				// If we've tried before then return the results
				if(s_AppSettingsException is not null) throw s_AppSettingsException;
				if(s_AllAppSettings is not null) return s_AllAppSettings;

				try
				{
					// These are the baseline values.
					// By setting this here any includes will be able to refer to them.
					s_AllAppSettings = LoadAppSettings();

					// Now layer on any additional includes
					s_AllAppSettings = LoadIncludes(s_AllAppSettings);

					return s_AllAppSettings;
				}
				catch(Exception e)
				{
					s_AppSettingsException = e;
					throw;
				}
			}
		}

		/// <summary>
		/// Returns the appSettings section from a configuration file.
		/// The values will not have variable expansion applied in order to be compatible with ConfigurationManager.AppSettings
		/// </summary>
		/// <returns></returns>
		private static NameValueCollection LoadAppSettings()
		{
			var settings = DoLegacyGetConfig("appSettings", false);
			return settings ?? new NameValueCollection();
		}

		private static ConnectionStringSettingsCollection LoadConnectionStrings()
		{
			var settings = new ConnectionStringSettingsCollection();

			XmlDocument? doc = s_ConfigDocument;
			if(doc is null || doc.DocumentElement is null) return settings;

			// It's optional
			XmlNode? section = doc.DocumentElement.SelectSingleNode("connectionStrings");
			if(section is null) return settings;

			foreach(XmlNode? node in section.SelectNodesOrEmpty("*"))
			{
				if(node is null) continue;

				switch(node.Name)
				{
					case "add":
					{
						var name = node.Attributes!.GetValueOrDefault("name", null);
						if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("invalid name in connectionString");
						if(settings[name] is not null) throw new ArgumentException($"database already exists: {name}");

						var providerName = node.Attributes!.GetValueOrDefault("providerName", "");
						var connectionString = node.Attributes!.GetValueOrDefault("connectionString", "");
						settings.Add(new(name, connectionString, providerName));

						break;
					}

					case "remove":
					{
						var name = node.Attributes!.GetValueOrDefault("name", null);
						if(name is not null) settings.Remove(name);
						break;
					}

					case "clear":
					{
						settings.Clear();
						break;
					}

					default:
						break;
				}
			}

			return settings;
		}
		
		/// <summary>
		/// Reads a section from the config file as xml
		/// </summary>
		/// <param name="system">The system to access, for example Arrow</param>
		/// <param name="sectionPath">The xpath to the section witin the system</param>
		/// <returns>The xml for the section, or null if not found</returns>
		/// <exception cref="System.ArgumentNullException">system is null</exception>
		/// <exception cref="System.ArgumentNullException">section is null</exception>
		public static XmlNode? GetSectionXml(string system, string sectionPath)
		{
			if(system==null) throw new ArgumentNullException("system");
			if(sectionPath==null) throw new ArgumentNullException("sectionPath");
			
			var doc=s_ConfigDocument;
			if(doc==null) return null;
			
			var systemNode=doc.DocumentElement!.SelectSingleNode(system);
			if(systemNode==null) return null;
			
			IConfigFile configFile=s_CurrentConfigFile;
			var searchRoot=GetSearchRoot(systemNode,sectionPath,configFile.Uri);
			if(searchRoot==null) return null;
			
			var node=searchRoot.SelectSingleNode(sectionPath);
			
			return node;
		}
		
		/// <summary>
		/// Returns the entire system xml configuration
		/// </summary>
		/// <param name="system">The system to access, for example Arrow</param>
		/// <returns>The xml for the system, or null if not found</returns>
		/// <exception cref="System.ArgumentNullException">system is null</exception>
		public static XmlNode? GetSystemXml(string system)
		{
            if(system == null) throw new ArgumentNullException("system");

            var doc = s_ConfigDocument;
            if(doc == null) return null;

            var node = doc.DocumentElement!.SelectSingleNode(system);
            return node;
        }
		
		/// <summary>
		/// Creates an object from a section
		/// </summary>
		/// <typeparam name="T">The type of object expected</typeparam>
		/// <param name="system">The system to access, for example Arrow</param>
		/// <param name="sectionPath">The xpath to the section witin the system</param>
		/// <returns>The object defined in the section, or null if the section does not exist</returns>
		/// <exception cref="System.ArgumentNullException">system is null</exception>
		/// <exception cref="System.ArgumentNullException">section is null</exception>
		public static T GetSectionObject<T>(string system, string sectionPath) where T:class
		{
			var node=GetSectionXml(system,sectionPath);			
			if(node==null) return default!;
			
			return XmlCreation.Create<T>(node);
		}
		
		/// <summary>
		/// Create a list of object held in a section
		/// </summary>
		/// <typeparam name="T">The type of object expected</typeparam>
		/// <param name="system">The system to access, for example Arrow</param>
		/// <param name="sectionPath">The xpath to the section witin the system</param>
		/// <param name="objectElementName">The name of the element within sectionPath that holds each object</param>
		/// <returns>A list of object. If nothing is found an empty list is returned</returns>
		public static List<T> GetSectionObjects<T>(string system, string sectionPath, string objectElementName) where T:class
		{
            if(objectElementName == null) throw new ArgumentNullException("objectElementName");

            var node = GetSectionXml(system, sectionPath);
            if(node == null) return new List<T>();

            return XmlCreation.CreateList<T>(node.SelectNodesOrEmpty(objectElementName));
        }
		
		/// <summary>
		/// Returns an object created by an IConfigurationSectionHandler instance.
		/// This approach is deprecated
		/// </summary>
		/// <typeparam name="T">The IConfigurationSectionHandler instance that will create the object</typeparam>
		/// <param name="system">The system to access, for example Arrow</param>
		/// <param name="sectionPath">The xpath to the section witin the system</param>
		/// <returns>An object, or null if the section does not exist</returns>
		/// <exception cref="System.ArgumentNullException">system is null</exception>
		/// <exception cref="System.ArgumentNullException">section is null</exception>
		public static object? GetSectionForHandler<T>(string system, string sectionPath) where T:IConfigurationSectionHandler,new()
		{
            var node = GetSectionXml(system, sectionPath);
            if(node == null) return null;

            IConfigurationSectionHandler handler = new T();
            object obj = handler.Create(null, null, node);

            return obj;
        }
		
		/// <summary>
		/// Works out where to get the section from by examining the xpath for the section.
		/// If the root of the section element has a "uri" attribute then this document will
		/// be loaded its DocumentElement returned
		/// </summary>
		/// <param name="systemNode">The system node (eg Arrow)</param>
		/// <param name="sectionPath">The section path (eg Arrow.Configuration/Foo)</param>
		/// <param name="baseUri">The uri of the current appconfig. This is used to resolve uris</param>
		/// <returns>The root element to start the search from</returns>
		private static XmlNode? GetSearchRoot(XmlNode systemNode, string sectionPath, Uri? baseUri)
		{
            // Work out which subsection we're looking at
            string rootSection = sectionPath;
            int pivot = sectionPath.IndexOf('/');
            if(pivot != -1) rootSection = sectionPath.Substring(0, pivot);

            var rootSectionNode = systemNode.SelectSingleNode(rootSection);
            if(rootSectionNode == null) return systemNode;

            var location = rootSectionNode.Attributes!.GetValueOrDefault("uri", null);
            if(location == null) return systemNode;

            location = TokenExpander.ExpandText(location);
            Uri uri = Accessor.ResolveRelative(baseUri, location);

            string allowMissingString = rootSectionNode.Attributes!.GetValueOrDefault("allowMissing", "false");

            bool allowMissing;
            bool.TryParse(allowMissingString, out allowMissing);

            XmlNode? searchRoot = null;

            try
            {
                var accessor = StorageManager.Get(uri);

                if(allowMissing && accessor.CanExists && accessor.Exists() == false)
                {
                    // The resource is allowed to be missing and we were able 
                    // to explicitly check that it wasn't there
                    searchRoot = null;
                }
                else
                {
                    XmlDocument resolvedDoc = StorageManager.Get(uri).ReadXmlDocument();
                    searchRoot = resolvedDoc.DocumentElement;
                }
            }
            catch
            {
                if(allowMissing == false) throw;
            }

            return searchRoot;
        }

		private static NameValueCollection? DoLegacyGetConfig(String path, bool expandValue)
		{
			if(string.IsNullOrWhiteSpace(path)) return null;

			XmlDocument? doc = s_ConfigDocument;
			if(doc is null || doc.DocumentElement is null) return null;

			var section = doc.DocumentElement.SelectSingleNode(path);
			if(section is null) return null;

			var data = new NameValueCollection();
			Apply(data, section, expandValue);

			return data;
		}

		private static void Apply(NameValueCollection target, XmlNode section, bool expandValue)
		{
			foreach(XmlNode? node in section.SelectNodesOrEmpty("*"))
			{
				if(node is null) continue;

				switch(node.Name)
				{
					case "add":
					{
						var key = node.Attributes!.GetValueOrDefault("key", null);
						if(key is null) throw new ArrowException("key is null");

						var value=node.Attributes!.GetValueOrDefault("value", null);
						if(value is null) throw new ArrowException("value is null");

						if(expandValue) value = TokenExpander.ExpandText(value);
						target[key] = value;
						break;
					}
					
					case "remove":
					{
						var key = node.Attributes!.GetValueOrDefault("key", null);
						if(key is null) throw new ArrowException("key is null");
						
						target.Remove(key);
						break;
					}
					
					case "clear":
						target.Clear();
						break;
						
					default:
						break;
					
				}
			}
		}

		private static NameValueCollection LoadIncludes(NameValueCollection startingPoint)
		{
			// We need to work with a copy to preservide the baseline
			var target = new NameValueCollection(startingPoint);
			ApplyAdditionalAppSettings(target);

			return target;
		}

		private static void ApplyAdditionalAppSettings(NameValueCollection target)
		{
			var node = GetSectionXml(ArrowSystem.Name, "Arrow.Configuration/AppSettings");
			if(node is null) return;

			var includeNodes = node.SelectNodesOrEmpty("Include");
			foreach(XmlNode? includeNode in includeNodes)
			{
				if(includeNode is null) continue;

				// By default we'll assume the user wants the file
				var optionalText = includeNode.Attributes!.GetValueOrDefault("optional", "false");
				if(bool.TryParse(optionalText, out var optional) == false) optional = false;

				var filename = includeNode.Attributes!.GetValueOrDefault("filename", null);
				if(filename is null) throw new ArrowException("no filename specified");

				var select = includeNode.Attributes!.GetValueOrDefault("select", null);

				// It's safe to do this, even if the filename uses a variable in the appSettings namespace.
				// However, the user will only see the variables from the app.config at this point
				var expandedFilename = TokenExpander.ExpandText(filename);
				ApplyFile(expandedFilename, optional, select, target, true);
			}
		}

		private static void ApplyFile(string filename, bool optional, string? select, NameValueCollection target, bool expandValue)
		{
			if(File.Exists(filename) == false && optional) return;

			try
			{
				var document = new XmlDocument();
				document.Load(filename);

				XmlNode? root = document.DocumentElement;
				if(root is null) throw new ArrowException($"no root element in {filename}");

				if(string.IsNullOrWhiteSpace(select) == false)
				{
					root = root.SelectSingleNode(select);
					if(root is null) throw new ArrowException($"select ({select}) did not find an element in {filename}");
				}

				Apply(target, root, expandValue);
			}
			catch(FileNotFoundException)
			{
				if(optional == false) throw;
			}
		}

		private static Lazy<ConnectionStringSettingsCollection> ResetDynamicState()
		{
			lock(s_AppSettingsSyncRoot)
			{
				s_AllAppSettings = null;
				s_AppSettingsException = null;
			}

			return new(() => LoadConnectionStrings());
		}
	}
}
