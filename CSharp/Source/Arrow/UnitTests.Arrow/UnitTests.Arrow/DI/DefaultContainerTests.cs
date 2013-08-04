using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Arrow.Configuration;
using Arrow.DI;

namespace UnitTests.Arrow.DI
{
	[TestFixture]
	public class DefaultContainerTests
	{
		private readonly DefaultContainer m_Container=new DefaultContainer();

		[TestFixtureSetUp]
		public void Setup()
		{
			m_Container.RegisterInstance<IContainer>(m_Container);
			m_Container.Register<IUserLookup,UserLookupStub>(Lifetime.Singleton);

			Type[] multiTypes={typeof(IFoo),typeof(IBar)};
			m_Container.Register(multiTypes,typeof(FooBar),Lifetime.Singleton);
		}

		[Test]
        public void SingletonLookup_ByGeneric()
        {
			var singleton1=m_Container.Resolve<IUserLookup>();
			Assert.IsNotNull(singleton1);

			var singleton2=m_Container.Resolve<IUserLookup>();
			Assert.IsNotNull(singleton2);

			Assert.AreSame(singleton1,singleton2);
        }

		[Test]
        public void SingletonLookup_ByType()
        {
			var singleton1=m_Container.Resolve(typeof(IUserLookup));
			Assert.IsNotNull(singleton1);

			var singleton2=m_Container.Resolve(typeof(IUserLookup));
			Assert.IsNotNull(singleton2);

			Assert.AreSame(singleton1,singleton2);
        }

		[Test]
		public void NonRegisteredType()
		{
			var manager1=m_Container.Resolve<UserManager>();
			Assert.IsNotNull(manager1);

			var manager2=m_Container.Resolve<UserManager>();
			Assert.IsNotNull(manager2);

			Assert.AreNotSame(manager1,manager2);
		}

		[Test]
		[ExpectedException(typeof(ContainerException))]
		public void NonRegisterdType_FromInterface()
		{
			m_Container.Resolve<INotImplemented>();
		}

		[Test]
		public void ConstructorTest()
		{
			var obj=m_Container.Resolve<ConstructorTest>();
			Assert.That(obj.Container,Is.Not.Null);
			Assert.AreSame(obj.Container,m_Container);
			
			Assert.That(obj.UserLookup,Is.Not.Null);
		}

		[Test]
		public void MultiTypeTest()
		{
			var foo=m_Container.Resolve<IFoo>();
			Assert.That(foo,Is.Not.Null);

			var bar=m_Container.Resolve<IBar>();
			Assert.That(bar,Is.Not.Null);

			Assert.AreSame(foo,bar);
		}

		[Test]
		public void PopulateFromXml_1()
		{
			var container=new DefaultContainer();

			var registrationNode=AppConfig.GetSectionXml(ArrowSystem.Name,"DI/Registration");
			container.RegisterFromXml(registrationNode);

			var foo=container.Resolve<IFoo>();
			Assert.That(foo,Is.Not.Null);

			var bar=container.Resolve<IBar>();
			Assert.That(bar,Is.Not.Null);

			Assert.AreSame(foo,bar);
		}

		[Test]
		public void PopulateFromXml_2()
		{
			var container=new DefaultContainer();
			container.RegisterInstance<IContainer>(container);

			var registrationNode=AppConfig.GetSectionXml(ArrowSystem.Name,"DI/Registration");
			container.RegisterFromXml(registrationNode);

			var ctest1=container.Resolve<ConstructorTest>();
			Assert.That(ctest1,Is.Not.Null);
			Assert.That(ctest1.Container,Is.Not.Null);
			Assert.That(ctest1.UserLookup,Is.Not.Null);

			var ctest2=container.Resolve<ConstructorTest>();
			Assert.AreNotSame(ctest1,ctest2);
		}
	}

	class UserManager
	{
		public UserManager(IUserLookup lookup, IContainer container)
		{
			Assert.IsNotNull(lookup);
			Assert.IsNotNull(container);
		}
	}

	interface INotImplemented
	{
		void Foo();
	}
}
