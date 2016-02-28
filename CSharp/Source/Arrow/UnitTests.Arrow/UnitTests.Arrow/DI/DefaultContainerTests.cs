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
			m_Container.RegisterInstance<IDIContainer>(m_Container);
			m_Container.Register<IUserLookup,UserLookupStub>(Lifetime.Singleton);

			Type[] multiTypes={typeof(IFoo),typeof(IBar)};
			m_Container.Register(multiTypes,typeof(FooBar),Lifetime.Singleton);
		}

		[Test]
		public void ResolveGeneric()
		{
			var foo=m_Container.Resolve<IFoo>();
			Assert.That(foo,Is.Not.Null);
		}

		[Test]
		public void ResolveType()
		{
			var foo=m_Container.Resolve(typeof(IFoo));
			Assert.That(foo,Is.Not.Null);
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
		public void Singletion_MultiInterface()
		{
			var container=new DefaultContainer();
			container.Register<IFoo,IBar,FooBar>(Lifetime.Singleton);

			var foo=container.Resolve<IFoo>();
			Assert.That(foo,Is.Not.Null);

			var bar=container.Resolve<IBar>();
			Assert.That(bar,Is.Not.Null);

			Assert.That(foo,Is.EqualTo(bar));
		}

		[Test]
		public void Transient_MultiInterface()
		{
			var container=new DefaultContainer();
			container.Register<IFoo,IBar,FooBar>(Lifetime.Transient);

			var foo=container.Resolve<IFoo>();
			Assert.That(foo,Is.Not.Null);

			var bar=container.Resolve<IBar>();
			Assert.That(bar,Is.Not.Null);

			Assert.That(foo,Is.Not.EqualTo(bar));
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
		public void UnregisteredInterface()
		{
			Assert.Throws<ContainerException>(()=>
			{
				var instance=m_Container.Resolve<IDisposable>();
			});
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

			var holder=container.Resolve<KeyHolder>();
			Assert.That(holder,Is.Not.Null);
			Assert.That(holder.Key,Is.EqualTo(8));
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

		[Test]
		public void PopulateFromXml_AssemblyScan()
		{
			var container=new DefaultContainer();
			container.RegisterInstance<IDIContainer>(container);

			var registrationNode=AppConfig.GetSectionXml(ArrowSystem.Name,"DI/AssemblyScan");
			container.RegisterFromXml(registrationNode);

			var holder=container.Resolve<KeyHolder>();
			Assert.That(holder,Is.Not.Null);
			Assert.That(holder.Key,Is.EqualTo(42));
		}

		[Test]
		public void RegisterBundleInAssembly()
		{
			var container=new DefaultContainer();
			container.RegisterBundlesInAssemby(typeof(DefaultContainerTests).Assembly);

			var holder=container.Resolve<KeyHolder>();
			Assert.That(holder,Is.Not.Null);
			Assert.That(holder.Key,Is.EqualTo(42));
		}

		[Test]
		public void RegisterBundleGeneric()
		{
			var container=new DefaultContainer();
			container.RegisterBundle<KeyBundle>();

			var holder=container.Resolve<KeyHolder>();
			Assert.That(holder,Is.Not.Null);
			Assert.That(holder.Key,Is.EqualTo(42));
		}

		[Test]
		public void RegisterBundleInstance()
		{
			var container=new DefaultContainer();
			container.RegisterBundle(new KeyBundle());

			var holder=container.Resolve<KeyHolder>();
			Assert.That(holder,Is.Not.Null);
			Assert.That(holder.Key,Is.EqualTo(42));
		}

		[Test]
		public void RegisterFactory_Singleton()
		{
			int calls=0;

			var container=new DefaultContainer();
			container.RegisterFactory<IFoo>(Lifetime.Singleton,di=>
			{
				calls++;
				return new TestFoo(null);
			});

			Assert.That(calls,Is.EqualTo(0));

			var instance1=container.Resolve<IFoo>();
			Assert.That(instance1,Is.Not.Null);
			Assert.That(calls,Is.EqualTo(1));

			var instance2=container.Resolve<IFoo>();
			Assert.That(instance2,Is.Not.Null);
			Assert.That(calls,Is.EqualTo(1));

			Assert.That(instance1,Is.SameAs(instance2));
		}

		[Test]
		public void RegisterFactory_Transient()
		{
			int calls=0;

			var container=new DefaultContainer();
			container.RegisterFactory<IFoo>(Lifetime.Transient,di=>
			{
				calls++;
				return new TestFoo(null);
			});

			Assert.That(calls,Is.EqualTo(0));

			var instance1=container.Resolve<IFoo>();
			Assert.That(instance1,Is.Not.Null);
			Assert.That(calls,Is.EqualTo(1));

			var instance2=container.Resolve<IFoo>();
			Assert.That(instance2,Is.Not.Null);
			Assert.That(calls,Is.EqualTo(2));

			Assert.That(instance1,Is.Not.SameAs(instance2));
		}

		[Test]
		public void RegisterFactory_UsingContainer()
		{
			var container=new DefaultContainer();
			container.Register<IFoo,IBar,FooBar>(Lifetime.Singleton);
			container.RegisterFactory<TakesFoo>(Lifetime.Transient,di=>
			{
				var foo=di.Resolve<IFoo>();
				Assert.That(foo,Is.Not.Null);
				
				return new TakesFoo(foo);
			});

			var takesFoo=container.Resolve<TakesFoo>();
			Assert.That(takesFoo,Is.Not.Null);
			Assert.That(takesFoo.Foo,Is.Not.Null);
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

	class KeyHolder
	{
		public KeyHolder(int key)
		{
			this.Key=key;
		}

		public int Key{get;private set;}
	}

	interface INotImplemented
	{
		void Foo();
	}


	public class KeyBundle : DependencyBundle
	{
		protected override IDIContainerRegister Register(IDIContainerRegister container)
		{
			return container.RegisterInstance(new KeyHolder(42));
		}
	}

	/// <summary>
	/// As this bundle isn't public it won't be picked up by the assembly scanning
	/// </summary>
	class IslandBundle : DependencyBundle
	{
		protected override IDIContainerRegister Register(IDIContainerRegister container)
		{
			return container.RegisterInstance(new KeyHolder(8));
		}
	}
}
