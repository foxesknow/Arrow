using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
		private static XmlDocument s_ConfigDocument;
		private static IConfigFile s_CurrentConfigFile;
		
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
		internal static XmlDocument ConfigDocument
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
			if(configFile==null) throw new ArgumentNullException("configFile");
			
			XmlDocument newDoc=configFile.LoadConfig();
			s_ConfigDocument=newDoc;
			s_CurrentConfigFile=configFile;
		}
		
		/// <summary>
		/// Returns the current config file loader in use
		/// </summary>
		public static IConfigFile ConfigFile
		{
			get{return s_CurrentConfigFile;}
		}
		
		/// <summary>
		/// Reads an "old-style" NameValueSectionHandler style configuration section.
		/// This method is intended to ease the transition to the other methods in this class
		/// </summary>
		/// <param name="path">The path to the section. May be null</param>
		/// <returns>A collection of values, or null if path does not resolve to a location in the xml</returns>
		public static NameValueCollection LegacyGetConfig(string path)
		{
			if(string.IsNullOrEmpty(path)) return null;
			
			XmlDocument doc=s_ConfigDocument;
			if(doc==null) return null;
			
			XmlNode section=doc.DocumentElement.SelectSingleNode(path);
			if(section==null) return null;
			
			NameValueSectionHandler handler=new NameValueSectionHandler();
			NameValueCollection data=new NameValueCollection();
			
			foreach(XmlNode node in section.SelectNodes("*"))
			{
				switch(node.Name)
				{
					case "add":
					{
						string key=node.Attributes["key"].Value;
						string value=node.Attributes["value"].Value;
						data[key]=TokenExpander.ExpandText(value);
						break;
					}
					
					case "remove":
					{
						string key=node.Attributes["key"].Value;
						data.Remove(key);
						break;
					}
					
					case "clear":
						data.Clear();
						break;
						
					default:
						break;
					
				}
			}
						
			return data;
		}
		
		/// <summary>
		/// Reads a section from the config file as xml
		/// </summary>
		/// <param name="system">The system to access, for example Arrow</param>
		/// <param name="sectionPath">The xpath to the section witin the system</param>
		/// <returns>The xml for the section, or null if not found</returns>
		/// <exception cref="System.ArgumentNullException">system is null</exception>
		/// <exception cref="System.ArgumentNullException">section is null</exception>
		public static XmlNode GetSectionXml(string system, string sectionPath)
		{
			if(system==null) throw new ArgumentNullException("system");
			if(sectionPath==null) throw new ArgumentNullException("sectionPath");
			
			XmlDocument doc=s_ConfigDocument;
			if(doc==null) return null;
			
			XmlNode systemNode=doc.DocumentElement.SelectSingleNode(system);
			if(systemNode==null) return null;
			
			IConfigFile configFile=s_CurrentConfigFile;
			XmlNode searchRoot=GetSearchRoot(systemNode,sectionPath,configFile.Uri);
			if(searchRoot==null) return null;
			
			XmlNode node=searchRoot.SelectSingleNode(sectionPath);
			
			return node;
		}
		
		/// <summary>
		/// Returns the entire system xml configuration
		/// </summary>
		/// <param name="system">The system to access, for example Arrow</param>
		/// <returns>The xml for the system, or null if not found</returns>
		/// <exception cref="System.ArgumentNullException">system is null</exception>
		public static XmlNode GetSystemXml(string system)
		{
			if(system==null) throw new ArgumentNullException("system");
			
			XmlDocument doc=s_ConfigDocument;
			if(doc==null) return null;
			
			XmlNode node=doc.DocumentElement.SelectSingleNode(system);			
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
			XmlNode node=GetSectionXml(system,sectionPath);			
			if(node==null) return default(T);
			
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
			if(objectElementName==null) throw new ArgumentNullException("objectElementName");
			
			XmlNode node=GetSectionXml(system,sectionPath);
			if(node==null) return new List<T>();
		
			return XmlCreation.CreateList<T>(node.SelectNodes(objectElementName));
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
		public static object GetSectionForHandler<T>(string system, string sectionPath) where T:IConfigurationSectionHandler,new()
		{
			XmlNode node=GetSectionXml(system,sectionPath);			
			if(node==null) return null;
			
			IConfigurationSectionHandler handler=new T();
			object obj=handler.Create(null,null,node);
			
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
		private static XmlNode GetSearchRoot(XmlNode systemNode, string sectionPath, Uri baseUri)
		{
			// Work out which subsection we're looking at
			string rootSection=sectionPath;			
			int pivot=sectionPath.IndexOf('/');
			if(pivot!=-1) rootSection=sectionPath.Substring(0,pivot);
			
			XmlNode rootSectionNode=systemNode.SelectSingleNode(rootSection);
			if(rootSectionNode==null) return systemNode;
			
			string location=rootSectionNode.Attributes.GetValueOrDefault("uri",null);
			if(location==null) return systemNode;
			
			location=TokenExpander.ExpandText(location);
			Uri uri=Accessor.ResolveRelative(baseUri,location);
			
			string allowMissingString=rootSectionNode.Attributes.GetValueOrDefault("allowMissing","false");
			
			bool allowMissing;
			bool.TryParse(allowMissingString,out allowMissing);
			
			XmlNode searchRoot=null;
			
			try
			{
				var accessor=StorageManager.Get(uri);
				
				if(allowMissing && accessor.CanExists && accessor.Exists()==false)
				{
					// The resource is allowed to be missing and we were able 
					// to explicitly check that it wasn't there
					searchRoot=null;
				}
				else
				{			
					XmlDocument resolvedDoc=StorageManager.Get(uri).ReadXmlDocument();
					searchRoot=resolvedDoc.DocumentElement;
				}
			}
			catch
			{
				if(allowMissing==false) throw;
			}
			
			return searchRoot;
		}
	}
}
