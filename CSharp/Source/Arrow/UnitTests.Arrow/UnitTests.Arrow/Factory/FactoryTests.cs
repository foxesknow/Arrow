using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow;
using Arrow.Factory;

using UnitTests.Arrow.Support;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace UnitTests.Arrow.Factory
{
	[TestFixture]
	public class FactoryTests
	{
		[Test]
		public void Register()
		{
			var factory=new SimpleFactory<Person>();
			factory.Register("person",typeof(Person));

			Assert.IsTrue(factory.Names.Contains("person"));
		}

		[Test]
		public void CreateSingle()
		{
			var factory=new SimpleFactory<Person>();
			factory.Register("person",typeof(Person));

			var instance=factory.Create("person");
			Assert.That(instance,Is.Not.Null);
		}

		[Test]
		public void CreateMultiple()
		{
			var factory=new SimpleFactory<Person>();
			factory.Register("person",typeof(Person));

			var instance1=factory.Create("person");
			Assert.That(instance1,Is.Not.Null);

			var instance2=factory.Create("person");
			Assert.That(instance2,Is.Not.Null);

			Assert.That(instance1,Is.Not.SameAs(instance2));
		}

		[Test]
		public void FailToCreate()
		{
			var factory=new SimpleFactory<Person>();
			factory.Register("person",typeof(Person));

			Assert.Throws<ArrowException>(()=>
			{
				var instance=factory.Create("bob");
			});
		}

		[Test]
		public void TryCreate()
		{
			var factory=new SimpleFactory<Person>();
			factory.Register("person",typeof(Person));

			var instance=factory.TryCreate("bob");
			Assert.That(instance,Is.Null);
		}
	}
}
