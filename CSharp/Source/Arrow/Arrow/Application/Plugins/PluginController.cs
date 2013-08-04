using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using Arrow.Configuration;
using Arrow.Storage;
using Arrow.Xml.ObjectCreation;

namespace Arrow.Application.Plugins
{
	/// <summary>
	/// Manages a group of plugins
	/// </summary>
	public class PluginController : IPluginDiscovery, IPlugin, IEnumerable<IPlugin>, IDisposable, IServiceProvider
	{
		private readonly object m_SyncRoot=new object();
		
		private readonly List<IPlugin> m_Plugins=new List<IPlugin>();
		private string m_PluginName;
		private volatile bool m_Started;
		
		private static readonly object s_SyncRoot=new object();
		private static PluginController s_SystemPlugins;
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		public PluginController()
		{
			m_PluginName="PluginController";
		}
		
		/// <summary>
		/// Starts all the plugins
		/// </summary>
		public void Start()
		{
			lock(m_SyncRoot)
			{
				if(!m_Started)
				{
					m_Plugins.ForEach(plugin=>
					{
						IPluginInitialize serviceInitialize=plugin as IPluginInitialize;
						if(serviceInitialize!=null) serviceInitialize.Initialize(this);
						plugin.Start();
					});
					
					m_Started=true;
					
					m_Plugins.ForEach(plugin=>
					{
						IPluginPostStart postStart=plugin as IPluginPostStart;
						if(postStart!=null) postStart.AllPluginsStarted(this);
					});
				}
			}
		}
		
		/// <summary>
		/// Stops all the plugins
		/// </summary>
		public void Stop()
		{
			lock(m_SyncRoot)
			{
				if(m_Started)
				{
					// Stop them in reverse order					
					foreach(var plugins in m_Plugins.AsEnumerable().Reverse())
					{
						plugins.Stop();
					}
					
					m_Started=false;
				}
			}
		}
		
		/// <summary>
		/// The name of the plugin controller
		/// </summary>
		/// <exception cref="System.ArgumentNullException">value is null</exception>
		public string Name
		{
			get{return m_PluginName;}
			set
			{
				if(m_PluginName==null) throw new ArgumentNullException("value");
				m_PluginName=value;
			}
		}
		
		/// <summary>
		/// Adds a new service
		/// </summary>
		/// <param name="service">The service to add</param>
		public void Add(IPlugin service)
		{
			if(service==null) throw new ArgumentNullException("service");
			if(m_Started) throw new InvalidOperationException("cannot add to controller once started");
			
			lock(m_SyncRoot)
			{
				m_Plugins.Add(service);
			}
		}
		
		/// <summary>
		/// Removes all plugins from the controller
		/// </summary>
		public void Clear()
		{
			if(m_Started) throw new InvalidOperationException("cannot clear controller once started");
		
			lock(m_SyncRoot)
			{
				m_Plugins.Clear();
			}
		}
		
		/// <summary>
		/// Removes a plugin
		/// </summary>
		/// <param name="plugin">The plugin to remove</param>
		/// <returns>true if a plugin was found and removed, otherwise false</returns>
		public bool Remove(IPlugin plugin)
		{
			if(m_Started) throw new InvalidOperationException("cannot remove once controller has started");
		
			lock(m_SyncRoot)
			{
				return m_Plugins.Remove(plugin);
			}
		}
		
		/// <summary>
		/// Searches for a plugin that matches a predicate
		/// </summary>
		/// <param name="predicate">The predicate to apply to each plugin</param>
		/// <returns>The first plugin to match the predicate, or null if no plugin matches</returns>
		public IPlugin Find(Predicate<IPlugin> predicate)
		{
			if(predicate==null) throw new ArgumentNullException("predicate");
			
			lock(m_SyncRoot)
			{
				return m_Plugins.Find(predicate);
			}
		}
		
		/// <summary>
		/// Searches for the first plugin to implements the specified type
		/// </summary>
		/// <typeparam name="T">The type the required plugin must implement</typeparam>
		/// <returns>The first plugin to match, or null if no plugin matches</returns>
		public T Find<T>() where T:IPlugin
		{
			Type type=typeof(T);
			return (T)GetService(type);
		}
		
		/// <summary>
		/// Searches for a serivce with a given name
		/// </summary>
		/// <param name="name">The name of the service to find</param>
		/// <returns>The first service that matches the name, otherwise null</returns>
		public IPlugin FindByName(string name)
		{
			if(name==null) throw new ArgumentNullException("name");
			
			return Find(service=>service.Name==name);
		}
		
		/// <summary>
		/// Returns all the plugins in a controller
		/// </summary>
		/// <returns>A sequence of service</returns>
		public IEnumerable<IPlugin> AllPlugins()
		{
			lock(m_SyncRoot)
			{
				return new List<IPlugin>(m_Plugins);
			}
		}
		
		/// <summary>
		///  Applies an action to every plugin
		/// </summary>
		/// <param name="action">The action to apply</param>
		public void ForEach(Action<IPlugin> action)
		{
			if(action==null) throw new ArgumentNullException("action");
			
			lock(m_SyncRoot)
			{
				m_Plugins.ForEach(action);
			}
		}
		
		/// <summary>
		/// Returns the number of plugins in the controller
		/// </summary>
		public int Count
		{
			get
			{
				lock(m_SyncRoot)
				{
					return m_Plugins.Count;
				}
			}
		}
		
		#region IServiceProvider Members

		/// <summary>
		/// Gets the service object of the specified type
		/// </summary>
		/// <param name="pluginType">The type of service object to get</param>
		/// <returns>A service object of the specified type, or null if no matching object found</returns>
		public object GetService(Type pluginType)
		{
			if(pluginType==null) throw new ArgumentNullException("pluginType");
			
			return Find(service=>pluginType.IsAssignableFrom(service.GetType()));
		}

		#endregion

		#region IEnumerable<IService> Members

		/// <summary>
		/// Reterns an enumerator for the plugins
		/// </summary>
		/// <returns>An enumerator</returns>
		public IEnumerator<IPlugin> GetEnumerator()
		{
			// We need to grab a copy, for thread safety
			List<IPlugin> plugins;
			
			lock(m_SyncRoot)
			{
				plugins=new List<IPlugin>(m_Plugins);
			}
			
			return plugins.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Stops all plugins and calls IDisposable.Dispose on any plugins that implement the interface
		/// </summary>
		public void Dispose()
		{
			lock(m_SyncRoot)
			{
				Stop();
				
				foreach(var plugin in m_Plugins)
				{
					IDisposable disposable=plugin as IDisposable;
					if(disposable!=null) disposable.Dispose();	
				}
				
				m_Plugins.Clear();
			}
		}

		#endregion
		
		/// <summary>
		/// Returns the systemwide plugin controller
		/// </summary>
		public static PluginController SystemServices
		{
			get
			{
				if(s_SystemPlugins==null)
				{
					lock(s_SyncRoot)
					{
						if(s_SystemPlugins==null)
						{
							
							XmlNode systemPluginsNode=AppConfig.GetSectionXml(ArrowSystem.Name,"Arrow.Services/SystemPlugins");
							if(systemPluginsNode!=null)
							{
								try
								{
									s_SystemPlugins=PluginController.FromXml(systemPluginsNode);
								}
								catch
								{
									// If we fail then create an empty controller
									s_SystemPlugins=new PluginController();
								}
							}
							else
							{
								s_SystemPlugins=new PluginController();
							}
						}
					}
				}
				
				return s_SystemPlugins;
			}
		}
		
		/// <summary>
		/// Creates a controller from an xml definition.
		/// The node must contain a sequence of "Plugin" elements
		/// </summary>
		/// <param name="pluginsRoot">The node containing the "Plugin" elements</param>
		/// <returns>A controller</returns>
		public static PluginController FromXml(XmlNode pluginsRoot)
		{
			if(pluginsRoot==null) throw new ArgumentNullException("pluginsRoot");
			
			PluginController controller=new PluginController();
			
			foreach(XmlNode pluginNode in pluginsRoot.SelectNodes("Plugin"))
			{
				var plugin=XmlCreation.Create<IPlugin>(pluginNode);
				controller.Add(plugin);
			}
			
			return controller;
		}
		
		/// <summary>
		/// Loads an xml document and generates a controller
		/// </summary>
		/// <param name="uri">The location of the xml document</param>
		/// <returns>A controller</returns>
		public static PluginController FromXml(Uri uri)
		{
			if(uri==null) throw new ArgumentNullException("uri");
		
			XmlDocument doc=StorageManager.Get(uri).ReadXmlDocument();
			return FromXml(doc.DocumentElement);
		}
	}
}
