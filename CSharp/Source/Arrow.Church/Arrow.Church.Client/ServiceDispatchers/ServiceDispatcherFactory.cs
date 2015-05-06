using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Arrow.Configuration;
using Arrow.Factory;

namespace Arrow.Church.Client.ServiceDispatchers
{
	static class ServiceDispatcherFactory
	{
		private static readonly SimpleFactory<ServiceDispatcherCreator> s_Factory=new SimpleFactory<ServiceDispatcherCreator>(StringComparer.OrdinalIgnoreCase);
		
		static ServiceDispatcherFactory()
		{
			LoadDefaults();			
		}
		
		private static void LoadDefaults()
		{
			bool loadDefaults=true;
			var node=AppConfig.GetSectionXml(ArrowSystem.Name,"Arrow.Church.Client/LoadDefaults");
			
			if(node!=null)
			{
				bool value;
				if(bool.TryParse(node.InnerText,out value)) loadDefaults=value;
			}
			
			if(loadDefaults) RegisteredTypeInstaller.LoadTypes("Arrow.Church.Client");
			
			var systemsNode=AppConfig.GetSectionXml(ArrowSystem.Name,"Arrow.Church.Client/Assemblies");
			if(systemsNode!=null)
			{
				foreach(XmlNode assemblyNode in systemsNode.SelectNodes("Assembly"))
				{
					string assemblyName=assemblyNode.InnerText;
					RegisteredTypeInstaller.LoadTypes(assemblyName);
				}
			}
		}
		
		public static void Register(string name, Type type)
		{
			s_Factory.Register(name,type);
		}
		
		public static ServiceDispatcherCreator TryCreate(string name)
		{
			return s_Factory.TryCreate(name);
		}
		
		public static bool IsPresent(string name)
		{
			bool present=false;
			
			Type type;
			present=s_Factory.TryGetType(name,out type);
			
			return present;
		}
	}
}
