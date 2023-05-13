using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using Arrow.Collections;
using Arrow.Factory;
using Arrow.Configuration;

namespace Arrow.Messaging
{
	/// <summary>
	/// Holds type information on all the MessagingSystem registered
	/// </summary>
	static class MessagingSystemFactory
	{
        private static readonly SimpleFactory<MessagingSystem> s_Factory = new SimpleFactory<MessagingSystem>(StringComparer.OrdinalIgnoreCase);

        static MessagingSystemFactory()
        {
            LoadDefaults();
        }

        private static void LoadDefaults()
        {
            bool loadDefaults = true;
            var node = AppConfig.GetSectionXml(ArrowSystem.Name, "Arrow.Messaging/LoadDefaults");

            if(node != null)
            {
                bool value;
                if(bool.TryParse(node.InnerText, out value)) loadDefaults = value;
            }

            if(loadDefaults) RegisteredTypeInstaller.LoadTypes("Arrow.Messaging.EMS");

            var systemsNode = AppConfig.GetSectionXml(ArrowSystem.Name, "Arrow.Messaging/Assemblies");
            if(systemsNode != null)
            {
                foreach(XmlNode assemblyNode in systemsNode.SelectNodes("Assembly"))
                {
                    string assemblyName = assemblyNode.InnerText;
                    RegisteredTypeInstaller.LoadTypes(assemblyName);
                }
            }
        }

        public static void Register(string name, Type type)
        {
            s_Factory.Register(name, type);
        }

        public static MessagingSystem TryCreate(string name)
        {
            return s_Factory.TryCreate(name);
        }

        public static bool IsPresent(string name)
        {
            bool present = false;

            Type type;
            present = s_Factory.TryGetType(name, out type);

            return present;
        }
    }
}
