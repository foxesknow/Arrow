using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.InsideOut;

using NUnit.Framework;

namespace UnitTests.Arrow.InsideOut
{
    [TestFixture]
    public class ArgumentTests : TestBase
    {
        [Test]
        public void Json()
        {
            Check(new BoolArgument("foo"){Value = true});
            Check(new Int32Argument("foo"){Value = 20});
            Check(new Int64Argument("foo"){Value = 40});
            Check(new DoubleArgument("foo"){Value = 80d});
            Check(new DecimalArgument("foo"){Value = 160m});
            
            Check(new TimeSpanArgument("foo"){Value = DateTime.Now.TimeOfDay});
            Check(new DateTimeArgument("foo"){Value = DateTime.Now});
            Check(new DateTimeArgument("foo"){Value = DateTime.UtcNow});
            Check(new TimeOnlyArgument("foo"){Value = TimeOnly.FromDateTime(DateTime.Now)});
            Check(new DateOnlyArgument("foo"){Value = DateOnly.FromDateTime(DateTime.Now)});
            
            Check(new StringArgument("foo"){Value = null});
            Check(new StringArgument("foo"){Value = "Jack"});
            
            Check(new SingleItemArgument("foo"){Value = null});
            Check(new SingleItemArgument("foo"){Value = new("Ben", 3)});

            Check(new MultipleItemsArgument("foo"));
            Check(new MultipleItemsArgument("foo"){Value = {new("Ben", 3), new("Sawyer", 13)} });
        }

        [Test]
        public void StringArgument_ValueOr()
        {
            var argument = new StringArgument("Name"){Value = "Jack"};
            Assert.That(argument.ValueOr("Ben"), Is.EqualTo("Jack"));
        }

        [Test]
        public void StringArgument_ValueOr_Null()
        {
            var argument = new StringArgument("Name");
            Assert.That(argument.ValueOr("Ben"), Is.EqualTo("Ben"));
        }

        [Test]
        [TestCase("Sawyer")]
        [TestCase(1)]
        [TestCase(1L)]
        public void SingleItem_MakeArgumentFromObject(object value)
        {
            var parameter = new SingleItemParameter("People")
            {
                Items =
                {
                    new("Jack", 0),
                    new("Sawyer", 1),
                    new("Ben", 2),
                }
            };

            var argument = parameter.MakeArgumentFromObject(value);
            Assert.That(argument, Is.Not.Null);
            Assert.That(argument.Value.Name, Is.EqualTo("Sawyer"));
            Assert.That(argument.Value.Ordinal, Is.EqualTo(1));
        }

        [Test]
        public void SingleItem_MakeArgumentFromObject_Null()
        {
            var parameter = new SingleItemParameter("People")
            {
                Items =
                {
                    new("Jack", 0),
                    new("Sawyer", 1),
                    new("Ben", 2),
                }
            };

            var argument = parameter.MakeArgumentFromObject(null);
            Assert.That(argument, Is.Not.Null);
            Assert.That(argument.Value, Is.Null);
        }

        [Test]
        public void SingleItem_MakeArgumentFromObject_BadType()
        {
            var parameter = new SingleItemParameter("People")
            {
                Items =
                {
                    new("Jack", 0),
                    new("Sawyer", 1),
                    new("Ben", 2),
                }
            };

            Assert.Catch(() => parameter.MakeArgumentFromObject(DateTime.Now));
        }

        [Test]
        [TestCase("Sawyer")]
        [TestCase(1)]
        [TestCase(1L)]
        public void MultipleItems_MakeArgumentFromObject(object value)
        {
            var parameter = new MultipleItemsParameter("People")
            {
                Items =
                {
                    new("Jack", 0),
                    new("Sawyer", 1),
                    new("Ben", 2),
                }
            };

            var argument = parameter.MakeArgumentFromObject(value);
            Assert.That(argument, Is.Not.Null);
            Assert.That(argument.Value.Count, Is.EqualTo(1));
            Assert.That(argument.Value[0].Name, Is.EqualTo("Sawyer"));
            Assert.That(argument.Value[0].Ordinal, Is.EqualTo(1));
        }

        [Test]
        public void MultipleItems_MakeArgumentFromObject_Null()
        {
            var parameter = new MultipleItemsParameter("People")
            {
                Items =
                {
                    new("Jack", 0),
                    new("Sawyer", 1),
                    new("Ben", 2),
                }
            };

            var argument = parameter.MakeArgumentFromObject(null);
            Assert.That(argument, Is.Not.Null);
            Assert.That(argument.Value.Count, Is.EqualTo(0));
        }

        [Test]
        public void MultipleItems_MakeArgumentFromObject_BadType()
        {
            var parameter = new MultipleItemsParameter("People")
            {
                Items =
                {
                    new("Jack", 0),
                    new("Sawyer", 1),
                    new("Ben", 2),
                }
            };

            Assert.Catch(() => parameter.MakeArgumentFromObject(DateTime.Now));
        }

        private void Check(Argument argument)
        {
            var roundTrip = RoundTrip(argument);

            Assert.That(argument.Name, Is.EqualTo(argument.Name));
            Assert.That(argument.AsObject(), Is.EqualTo(roundTrip.AsObject()));
            Assert.That(argument.Type(), Is.EqualTo(roundTrip.Type()));
        }
    }
}
