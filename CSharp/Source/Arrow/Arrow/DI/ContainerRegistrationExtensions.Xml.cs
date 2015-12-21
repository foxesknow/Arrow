using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using Arrow.Xml.ObjectCreation;

namespace Arrow.DI
{
	public static partial class ContainerRegistrationExtensions
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
		public static IDIContainerRegister RegisterFromXml(this IDIContainerRegister container, XmlNode root)
		{
			if(root==null) throw new ArgumentNullException("root");
			
			var itemsToRegister=XmlCreation.CreateList<RegistrationItem>(root.SelectNodes("Register"));
			foreach(var item in itemsToRegister)
			{
				if(item.ConcreteType==null) throw new ContainerException("no concrete type specified");

				if(item.ExposedTypes.Count==0)
				{
					// Assume that the concrete type is the exposed type
					item.ExposedTypes.Add(item.ConcreteType);
				}
				
				container.Register(item.ExposedTypes,item.ConcreteType,item.Lifetime);
			}

			return container;
		}

		class RegistrationItem : ICustomXmlInitialization
		{
			public RegistrationItem()
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
	}
}
