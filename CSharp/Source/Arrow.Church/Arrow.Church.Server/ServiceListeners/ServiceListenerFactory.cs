using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Configuration;
using Arrow.Factory;

namespace Arrow.Church.Server.ServiceListeners
{
	static class ServiceListenerFactory
	{
		private static readonly SimpleFactory<ServiceListenerCreator> s_Factory=new SimpleFactory<ServiceListenerCreator>(StringComparer.OrdinalIgnoreCase);
		
		static ServiceListenerFactory()
		{
			LoadDefaults();			
		}
		
		private static void LoadDefaults()
		{
			bool loadDefaults=true;
			var node=AppConfig.GetSectionXml(ArrowSystem.Name,"Arrow.Church.Server/LoadDefaults");
			
			if(node!=null)
			{
				bool value;
				if(bool.TryParse(node.InnerText,out value)) loadDefaults=value;
			}
			
			if(loadDefaults) RegisteredTypeInstaller.LoadTypes(typeof(ServiceListenerFactory).Assembly);		
		}
		
		public static void Register(string name, Type type)
		{
			s_Factory.Register(name,type);
		}
		
		public static ServiceListenerCreator TryCreate(string name)
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
