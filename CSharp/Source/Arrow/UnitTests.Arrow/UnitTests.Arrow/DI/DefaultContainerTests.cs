﻿using System;
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
			m_Container.RegisterInstance<IDIContainer>(m_Container);
			m_Container.Register<IUserLookup,UserLookupStub>(Lifetime.Singleton);

			Type[] multiTypes={typeof(IFoo),typeof(IBar)};
			m_Container.Register(multiTypes,typeof(FooBar),Lifetime.Singleton);
		}

		[Test]
		public void ScopeTest()
		{
			var parent=new DefaultContainer();
			parent.RegisterInstance<IDIContainer>(parent);
			parent.Register<IFoo,FooBar>(Lifetime.Singleton);

			var child=parent.NewScope();
			child.RegisterInstance<IDIContainer>(child);

			var container=m_Container.NewScope();
			container.RegisterInstance<IDIContainer>(container);

			Assert.That(child.Resolve<IDIContainer>(), Is.Not.EqualTo(parent));
			Assert.That(child.Resolve<IDIContainer>(), Is.EqualTo(child));

			// This should call down to the parent scope
			var lookupParent=parent.Resolve<IFoo>();
			var lookupChild=child.Resolve<IFoo>();
			
			Assert.That(lookupParent,Is.EqualTo(lookupChild));
		}

		[Test]
		public void ScopeTest_CallToChild1()
		{
			var parent=new DefaultContainer();
			parent.RegisterInstance<IDIContainer>(parent);
			parent.Register<IFoo,FooBar>(Lifetime.Singleton);

			var child=parent.NewScope();
			child.RegisterInstance<IDIContainer>(child);
			child.Register<IFoo,FooBar>(Lifetime.Singleton);

			var parentFoo1=parent.Resolve<TakesFoo>();
			var parentFoo2=parent.Resolve<TakesFoo>();
			Assert.That(parentFoo1.Foo,Is.EqualTo(parentFoo2.Foo));

			var childFoo1=child.Resolve<TakesFoo>();
			var childFoo2=child.Resolve<TakesFoo>();
			Assert.That(childFoo1.Foo,Is.EqualTo(childFoo2.Foo));

			// The parent foo should be different to the child foo
			Assert.That(childFoo1.Foo,Is.Not.EqualTo(parentFoo1.Foo));
		}

		[Test]
		public void ScopeTest_CallToChild2()
		{
			var parent=new DefaultContainer();
			parent.Register<IFoo,FooBar>(Lifetime.Singleton);

			var child=parent.NewScope();
			child.Register<TakesFoo,TakesFoo>(Lifetime.Singleton);

			var takesFoo=child.Resolve<TakesFoo>();
		}

		[Test]
		public void ScopeTest_CallToChild3()
		{
			var parent=new DefaultContainer();
			parent.Register<TakesFoo,TakesFoo>(Lifetime.Singleton);

			var child=parent.NewScope();
			child.Register<IFoo,TestFoo>(Lifetime.Singleton);
			child.Register<IBar,FooBar>(Lifetime.Singleton);

			// TakesFoo will be created in the parent, but its dependent types live in the child
			var takesFoo=child.Resolve<TakesFoo>();
			Assert.That(takesFoo.Foo,Is.TypeOf<TestFoo>());
			Assert.That(((TestFoo)takesFoo.Foo).Bar,Is.TypeOf<FooBar>());
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
			container.RegisterInstance<IDIContainer>(container);

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

	class TakesFoo
	{
		public TakesFoo(IFoo foo)
		{
			this.Foo=foo;
		}

		public IFoo Foo{get;set;}
	}

	class TestFoo : IFoo
	{
		private IBar m_Bar;

		public TestFoo(IBar bar)
		{
			m_Bar=bar;
		}

		public IBar Bar
		{
			get{return m_Bar;}
		}

		public void HandleFoo()
		{
			throw new NotImplementedException();
		}
	}

	class UserManager
	{
		public UserManager(IUserLookup lookup, IDIContainer container)
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
