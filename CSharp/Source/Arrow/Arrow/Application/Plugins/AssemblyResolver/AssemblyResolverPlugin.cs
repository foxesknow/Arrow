using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;
using System.IO;

using Arrow.Logging;
using Arrow.Xml.ObjectCreation;
using System.Threading.Tasks;

#nullable disable

namespace Arrow.Application.Plugins.AssemblyResolver
{
	/// <summary>
	/// Resolves assemblies by looking through a series of paths until a match is found
	/// Paths are searched in the order they are added
	/// </summary>
	public class AssemblyResolverPlugin : Plugin, ICustomXmlInitialization
	{
		private static readonly ILog Log=LogManager.GetLog<AssemblyResolverPlugin>();
	
		private List<string> m_Paths=new List<string>();
		private bool m_LogResolveCalls;
		
		/// <summary>
		/// Adds a new path to the assembly search list
		/// </summary>
		/// <param name="path"></param>
		public void AddPath(string path)
		{
            if(path == null) throw new ArgumentNullException("path");

            m_Paths.Add(path);
        }
		
		/// <summary>
		/// Indicates if calls to resolve an assembly should be logged
		/// </summary>
		public bool LogResolveCalls
		{
			get{return m_LogResolveCalls;}
			set{m_LogResolveCalls = value;}
		}
	
		private Assembly AssemblyResolve(object sender, ResolveEventArgs args)
		{
            Assembly assembly = null;

            string bareName = args.Name;
            if(m_LogResolveCalls) Log.InfoFormat("AssemblyResolverPlugin - resolving {0}", bareName);

            string filename = string.Format("{0}.dll", args.Name);

            foreach(string path in m_Paths)
            {
                string fullname = Path.Combine(path, filename);
                if(File.Exists(fullname))
                {
                    try
                    {
                        assembly = Assembly.LoadFile(fullname);
                        if(m_LogResolveCalls) Log.InfoFormat("AssemblyResolverPlugin - {0} resolved to {1}", bareName, fullname);

                        break;
                    }
                    catch(Exception e)
                    {
                        Log.ErrorFormat("AssemblyResolverPlugin - found assembly {0} put could not load it. {1}", bareName, e.Message);
                        return null; // NOTE: Early return
                    }
                }
            }

            if(m_LogResolveCalls) Log.WarnFormat("AssemblyResolverPlugin - could not resolve {0}", bareName);

            return assembly;
        }
	
		/// <summary>
		/// Starts the plugin
		/// </summary>
		protected internal override ValueTask Start()
		{
			AppDomain.CurrentDomain.AssemblyResolve+=AssemblyResolve;
			return default;
		}

		/// <summary>
		/// Stops the plugin
		/// </summary>
		protected internal override ValueTask Stop()
		{
			AppDomain.CurrentDomain.AssemblyResolve -= AssemblyResolve;
			return default;
		}

		/// <summary>
		/// The name of the plugin
		/// </summary>
		public override string Name
		{
			get{return "AssemblyResolver";}
		}

		void ICustomXmlInitialization.InitializeObject(XmlNode rootNode, ICustomXmlCreation factory)
		{
            factory.Apply(this, rootNode);

            foreach(XmlNode pathNode in rootNode.SelectNodes("Path"))
            {
                string path = factory.Create<string>(pathNode);
                AddPath(path);
            }
        }

	}
}
