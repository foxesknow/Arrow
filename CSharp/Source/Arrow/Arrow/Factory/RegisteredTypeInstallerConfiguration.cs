using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Arrow.Xml;
using Arrow.Xml.ObjectCreation;

namespace Arrow.Factory
{
	/// <summary>
	/// Holds the filenames of a set of assemblies to register
	/// </summary>
	public class RegisteredTypeInstallerConfiguration
	{
		private readonly List<string> m_Assemblies = new List<string>();
		
		/// <summary>
		///  The assemblies to register
		/// </summary>
		public List<String> Assemblies
		{
			get{return m_Assemblies;}
		}
		
		/// <summary>
		/// Registers each assembly by calling RegisteredTypeInstaller.LoadTypes
		/// </summary>
		public void Apply()
		{
			foreach(string assembly in m_Assemblies)
			{
				RegisteredTypeInstaller.LoadTypes(assembly);
			}
		}
		
		/// <summary>
		/// Create a RegisteredTypeInstallerConfiguration instance from an xml node
		/// </summary>
		/// <param name="node">The node containing a series of <b>Assembly</b> elements</param>
		/// <returns>A RegisteredTypeInstallerConfiguration instance</returns>
		public static RegisteredTypeInstallerConfiguration FromXml(XmlNode node)
		{
            if(node == null) throw new ArgumentNullException(nameof(node));

            List<string> assemblies = XmlCreation.CreateList<string>(node.SelectNodesOrEmpty("Assembly"));

            RegisteredTypeInstallerConfiguration config = new RegisteredTypeInstallerConfiguration();
            config.Assemblies.AddRange(assemblies);

            return config;
        }
	}
}
