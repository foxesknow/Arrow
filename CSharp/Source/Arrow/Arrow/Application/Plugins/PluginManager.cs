using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using Arrow.Configuration;
using Arrow.Storage;
using Arrow.Threading.Tasks;
using Arrow.Xml;
using Arrow.Xml.ObjectCreation;

namespace Arrow.Application.Plugins
{
	/// <summary>
	/// Manages a group of plugins.
	/// 
	/// This class is for use by the framework and should not be called directly
	/// </summary>
	internal class PluginManager : IPluginDiscovery, IEnumerable<Plugin>, IAsyncDisposable, IServiceProvider
	{
		private readonly ConcurrentQueue<Plugin> m_Plugins = new();
		private string m_PluginName;
		private volatile bool m_Started;
		
		private static readonly object s_SyncRoot=new object();
		private static PluginManager? s_SystemPlugins;
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		public PluginManager()
		{
			m_PluginName = "PluginController";
		}
		
		/// <summary>
		/// Starts all the plugins
		/// </summary>
		internal async ValueTask Start()
		{
			if(!m_Started)
			{
				foreach(var plugin in m_Plugins)
				{
					if(plugin is IPluginInitialize serviceInitialize) 
					{
						await serviceInitialize.Initialize(this).ContinueOnAnyContext();
					}
					
					await plugin.Start().ContinueOnAnyContext();
				}

				m_Started=true;
					
				foreach(var plugin in m_Plugins)
				{
					if(plugin is IPluginPostStart postStart) 
					{
						await postStart.AllPluginsStarted(this).ContinueOnAnyContext();
					}
				}
			}
		}
		
		/// <summary>
		/// Stops all the plugins
		/// </summary>
		internal async ValueTask Stop()
		{
			if(m_Started)
			{
				// Stop them in reverse order					
				foreach(var plugins in m_Plugins.AsEnumerable().Reverse())
				{
					await plugins.Stop().ContinueOnAnyContext();
				}
					
				m_Started=false;
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
				if(value == null) throw new ArgumentNullException("value");
				m_PluginName = value;
			}
		}
		
		/// <summary>
		/// Adds a new service
		/// </summary>
		/// <param name="service">The service to add</param>
		internal void Add(Plugin service)
		{
			if(service is null) throw new ArgumentNullException("service");
			if(m_Started) throw new InvalidOperationException("cannot add to controller once started");
			
			m_Plugins.Enqueue(service);
		}
		
		/// <summary>
		/// Removes all plugins from the controller
		/// </summary>
		internal void Clear()
		{
			if(m_Started) throw new InvalidOperationException("cannot clear controller once started");
		
			m_Plugins.Clear();
		}
		
		/// <summary>
		/// Searches for a plugin that matches a predicate
		/// </summary>
		/// <param name="predicate">The predicate to apply to each plugin</param>
		/// <returns>The first plugin to match the predicate, or null if no plugin matches</returns>
		public Plugin? Find(Func<Plugin, bool> predicate)
		{
			if(predicate is null) throw new ArgumentNullException("predicate");
			
			return m_Plugins.FirstOrDefault(predicate);
		}
		
		/// <summary>
		/// Searches for the first plugin to implements the specified type
		/// </summary>
		/// <typeparam name="T">The type the required plugin must implement</typeparam>
		/// <returns>The first plugin to match, or null if no plugin matches</returns>
		public T? Find<T>() where T : class
		{
			Type type = typeof(T);
			return (T?)GetService(type);
		}
		
		/// <summary>
		/// Searches for a serivce with a given name
		/// </summary>
		/// <param name="name">The name of the service to find</param>
		/// <returns>The first service that matches the name, otherwise null</returns>
		public Plugin? FindByName(string name)
		{
			if(name is null) throw new ArgumentNullException(nameof(name));
			
			return Find(service => service.Name == name);
		}
		
		/// <summary>
		/// Returns the number of plugins in the controller
		/// </summary>
		public int Count
		{
			get{return m_Plugins.Count;}
		}
		
		/// <summary>
		/// Gets the plugin object of the specified type
		/// </summary>
		/// <param name="pluginType">The type of service object to get</param>
		/// <returns>A service object of the specified type, or null if no matching object found</returns>
		public object? GetService(Type pluginType)
		{
			if(pluginType is null) throw new ArgumentNullException("pluginType");
			
			return Find(service => pluginType.IsAssignableFrom(service.GetType()));
		}

		/// <summary>
		/// Reterns an enumerator for the plugins
		/// </summary>
		/// <returns>An enumerator</returns>
		public IEnumerator<Plugin> GetEnumerator()
		{
			return m_Plugins.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Stops all plugins.
		/// 
		/// If the plugin implements IDisposable then Dispose() is called.
		/// Otherwise if the plugin implements IAsyncDisposable then DisposeAsync() is called
		/// </summary>
		public async ValueTask DisposeAsync()
		{
			await Stop().ContinueOnAnyContext();
				
			foreach(var plugin in m_Plugins)
			{
				if(plugin is IDisposable disposer) 
				{
					disposer.Dispose();	
				}
				else if(plugin is IAsyncDisposable asyncDisposer)
				{
					await asyncDisposer.DisposeAsync().ContinueOnAnyContext();
				}
			}
				
			m_Plugins.Clear();
		}

		/// <summary>
		/// Returns the systemwide plugin controller
		/// </summary>
		internal static PluginManager SystemServices
		{
			get
			{
                if(s_SystemPlugins == null)
                {
                    lock(s_SyncRoot)
                    {
                        if(s_SystemPlugins == null)
                        {

                            var systemPluginsNode = AppConfig.GetSectionXml(ArrowSystem.Name, "Arrow.Plugins/SystemPlugins");
                            if(systemPluginsNode != null)
                            {
                                try
                                {
                                    s_SystemPlugins = PluginManager.FromXml(systemPluginsNode);
                                }
                                catch
                                {
                                    // If we fail then create an empty controller
                                    s_SystemPlugins = new PluginManager();
                                }
                            }
                            else
                            {
                                s_SystemPlugins = new PluginManager();
                            }

                            s_SystemPlugins.Name = "SystemPlugins";
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
		internal static PluginManager FromXml(XmlNode pluginsRoot)
		{
            if(pluginsRoot == null) throw new ArgumentNullException("pluginsRoot");

            PluginManager controller = new PluginManager();

            foreach(XmlNode? pluginNode in pluginsRoot.SelectNodesOrEmpty("Plugin"))
            {
                var plugin = XmlCreation.Create<Plugin>(pluginNode!);
                controller.Add(plugin);
            }

            return controller;
        }
		
		/// <summary>
		/// Loads an xml document and generates a controller
		/// </summary>
		/// <param name="uri">The location of the xml document</param>
		/// <returns>A controller</returns>
		internal static PluginManager FromXml(Uri uri)
		{
            if(uri == null) throw new ArgumentNullException("uri");

            XmlDocument doc = StorageManager.Get(uri).ReadXmlDocument();
            return FromXml(doc.DocumentElement!);
        }
	}
}
