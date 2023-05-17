using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

using Arrow.Configuration;
using Arrow.Execution;
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
		private readonly ConcurrentQueue<Plugin> m_Plugins;
		
		private bool m_Started;
		
		private readonly AsyncLock m_SyncRoot = new();
		
		private static readonly object s_SyncRoot=new object();
		private static PluginManager? s_SystemPlugins;
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		public PluginManager(IEnumerable<Plugin> plugins)
		{
			m_Plugins = new(plugins);
		}
		
		/// <summary>
		/// Starts all the plugins
		/// </summary>
		internal async ValueTask Start()
		{
			using(await m_SyncRoot)
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

					m_Started = true;
					
					foreach(var plugin in m_Plugins)
					{
						if(plugin is IPluginPostStart postStart) 
						{
							await postStart.AllPluginsStarted(this).ContinueOnAnyContext();
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Stops all plugins, if started, and disposes of them
		/// 
		/// If the plugin implements IDisposable then Dispose() is called.
		/// Otherwise if the plugin implements IAsyncDisposable then DisposeAsync() is called
		/// </summary>
		public async ValueTask DisposeAsync()
		{
			using(await m_SyncRoot)
			{
				// Shut them down in the opposite order to how we started them
				foreach(var plugin in m_Plugins.AsEnumerable().Reverse())
				{
					if(m_Started)
					{
						await plugin.Stop().ContinueOnAnyContext();
					}

					await Disposable.TryDisposeAsync(plugin);
				}
				
				m_Plugins.Clear();
			}
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
			if(pluginType is null) throw new ArgumentNullException(nameof(pluginType));
			
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
		/// Returns the systemwide plugin manager
		/// </summary>
		internal static PluginManager GetSystemPlugins()
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
                            s_SystemPlugins = new PluginManager(Array.Empty<Plugin>());
                        }
                    }
                    else
                    {
                        s_SystemPlugins = new PluginManager(Array.Empty<Plugin>());
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
            if(pluginsRoot == null) throw new ArgumentNullException(nameof(pluginsRoot));

            var plugins = new List<Plugin>();

            foreach(XmlNode? pluginNode in pluginsRoot.SelectNodesOrEmpty("Plugin"))
            {
                var plugin = XmlCreation.Create<Plugin>(pluginNode!);
                plugins.Add(plugin);
            }

			return new PluginManager(plugins);
        }
		
		/// <summary>
		/// Loads an xml document and generates a controller
		/// </summary>
		/// <param name="uri">The location of the xml document</param>
		/// <returns>A controller</returns>
		internal static PluginManager FromXml(Uri uri)
		{
            if(uri == null) throw new ArgumentNullException(nameof(uri));

            var doc = StorageManager.Get(uri).ReadXmlDocument();
            return FromXml(doc.DocumentElement!);
        }
	}
}
