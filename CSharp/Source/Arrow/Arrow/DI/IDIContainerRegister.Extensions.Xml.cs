using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Arrow.Reflection;
using Arrow.Xml.ObjectCreation;

namespace Arrow.DI
{
	public static partial class IDIContainerRegisterExtensions
	{
		/// <summary>
		/// Registers DI items from xml
		/// 
		/// </summary>
		/// <remarks>
		/// The root node must contain a series of "Register" elements which descibe what should be registered.
		/// </remarks>
		/// <example>
		/// <![CDATA[
		/// <Register lifetime="Singleton">
		///		<Concrete type="UnitTests.Arrow.DI.FooBar, UnitTests.Arrow" />
		///		<Expose type="UnitTests.Arrow.DI.IFoo, UnitTests.Arrow" />
		///		<Expose type="UnitTests.Arrow.DI.IBar, UnitTests.Arrow" />
		/// </Register>
		/// ]]>
		/// </example>
		/// <param name="container">The container the items will be registered into</param>
		/// <param name="root"></param>
		/// <returns>The container to use in subsequent calls</returns>
		public static IDIContainerRegister RegisterFromXml(this IDIContainerRegister container, XmlNode root)
		{
			if(root==null) throw new ArgumentNullException("root");
			
			var registerNodes=root.SelectNodes("Register|RegisterBundles|RegisterBundlesInAssembly");
			foreach(XmlNode node in registerNodes)
			{
				switch(node.Name)
				{
					case "Register":
					{
						var item=XmlCreation.Create<RegisterItem>(node);
						if(item.ConcreteType==null) throw new ContainerException("no concrete type specified");

						if(item.ExposedTypes.Count==0)
						{
							// Assume that the concrete type is the exposed type
							item.ExposedTypes.Add(item.ConcreteType);
						}
				
						container=container.Register(item.ExposedTypes,item.ConcreteType,item.Lifetime);

						break;
					}

					case "RegisterBundles":
					{
						var registerBundles=XmlCreation.Create<RegisterBundlesItem>(node);
						
						foreach(var bundle in registerBundles.Bundles)
						{
							if(bundle!=null)
							{
								container=bundle.RegisterBundle(container);
							}
						}

						break;
					}

					case "RegisterBundlesInAssembly":
					{
						var registerBundlesInAssembly=XmlCreation.Create<RegisterBundlesInAssemblyItem>(node);

						foreach(var assemblyName in registerBundlesInAssembly.Assemblies)
						{
							var assembly=TypeResolver.LoadAssembly(assemblyName);
							container=container.RegisterBundlesInAssemby(assembly);
						}

						break;
					}

					default:
						break;
				}
			}

			return container;
		}

		class RegisterItem : ICustomXmlInitialization
		{
			public RegisterItem()
			{
				this.ExposedTypes=new List<Type>();
				this.Lifetime=DI.Lifetime.Transient;
			}

			public List<Type> ExposedTypes{get;private set;}
			public Type ConcreteType{get;set;}
			public Lifetime Lifetime{get;set;}


			public void InitializeObject(XmlNode rootNode, CustomXmlCreation factory)
			{
				foreach(XmlNode node in rootNode.SelectNodes("Concrete"))
				{
					this.ConcreteType=factory.CreateType(node);
				}

				foreach(XmlNode node in rootNode.SelectNodes("Expose"))
				{
					var type=factory.CreateType(node);
					this.ExposedTypes.Add(type);
				}

				foreach(XmlNode node in rootNode.SelectNodes("Lifetime|@lifetime"))
				{
					var lifetime=factory.Create<Lifetime>(node);
					this.Lifetime=lifetime;
				}
			}

		}

		class RegisterBundlesItem
		{
			public RegisterBundlesItem()
			{
				this.Bundles=new List<DependencyBundle>();
			}

			public List<DependencyBundle> Bundles{get;private set;}
		}

		class RegisterBundlesInAssemblyItem
		{
			public RegisterBundlesInAssemblyItem()
			{
				this.Assemblies=new List<string>();
			}

			public List<string> Assemblies{get;private set;}
		}
	}
}
