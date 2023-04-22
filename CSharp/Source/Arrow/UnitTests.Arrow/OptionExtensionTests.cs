using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow;

using NUnit.Framework;

#nullable enable

namespace UnitTests.Arrow
{
    [TestFixture]
    public class OptionExtensionTests
    {
        [Test]
        public void Flatten()
        {
            var inner = Option.Some("Hello");
            var outer = Option.Some(inner);
            Assert.That(outer.Value(), Is.EqualTo(inner));

            var flattened = outer.Flatten();
            Assert.That(flattened, Is.EqualTo(inner));
        }

        [Test]
        public void ToNullable_Value()
        {
            Option<int> age = 10;
            var nullableAge = age.ToNullable();
            Assert.That(nullableAge.HasValue, Is.True);
            Assert.That(nullableAge!.Value, Is.EqualTo(10));
        }

        [Test]
        public void ToNullable_Value_Null()
        {
            Option<int> age = Option.None;
            var nullableAge = age.ToNullable();
            Assert.That(nullableAge.HasValue, Is.False);
        }

        [Test]
        public void ToNullable_Reference()
        {
            Option<string> name = "Robert";
            var nullableName = name.ToNullable();
            Assert.That(nullableName, Is.Not.Null);
            Assert.That(nullableName!, Is.EqualTo("Robert"));
        }

        [Test]
        public void ToNullable_Reference_Null()
        {
            Option<string> name = Option.None;
            var nullableName = name.ToNullable();
            Assert.That(nullableName, Is.Null);
        }

        [Test]
        public void OrElse()
        {
            Option<int> x = 10;
            var y = x.OrElse(20);
            Assert.That(y.Value(), Is.EqualTo(10));
        }

        [Test]
        public void OrElse_None()
        {
            Option<int> x = default;
            var y = x.OrElse(20);
            Assert.That(y.Value(), Is.EqualTo(20));
        }

        [Test]
        public void OrElse_None_None()
        {
            Option<int> x = default;
            var y = x.OrElse(Option.None);
            Assert.That(y, Is.EqualTo(Option.None));
        }

        [Test]
        public void OrElse_Func()
        {
            Option<int> x = default;
            var y = x.OrElse(static () => 20);
            Assert.That(y.Value(), Is.EqualTo(20));
        }

        [Test]
        public void OrElse_Func_NotCalled()
        {
            bool called = false;

            Option<int> x = 10;
            var y = x.OrElse(() => {called = true; return 20;});
            Assert.That(y.Value(), Is.EqualTo(10));
            Assert.That(called, Is.False);
        }

        [Test]
        public void OrElse_Func_State()
        {
            var answer = 20;

            Option<int> x = default;
            var y = x.OrElse(answer, static state => state);
            Assert.That(y.Value(), Is.EqualTo(20));
        }

        [Test]
        public void OrElse_Func_State_NotCalled()
        {
            var answer = 20;
            bool called = false;

            Option<int> x = default;
            var y = x.OrElse(answer, state => {called = true; return state;});
            Assert.That(y.Value(), Is.EqualTo(20));
            Assert.That(called, Is.True);
        }

        [Test]
        public void SelectMany_Linq_1()
        {
            Option<int> x = new(10);
            Option<int> y = new(20);

            var total = from a in x
                        from b in y
                        select a + b;

            Assert.That(total.Value(), Is.EqualTo(30));
        }

        [Test]
        public void SelectMany_Linq_2()
        {
            Option<int> x = new(10);
            Option<int> y = new(20);
            Option<int> z = new(30);

            var total = from a in x
                        from b in y
                        from c in z
                        select a + b + c;

            Assert.That(total.Value(), Is.EqualTo(60));
        }

        [Test]
        public void SelectMany_Linq_None_1()
        {
            Option<int> x = new(10);
            Option<int> y = Option.None;

            var total = from a in x
                        from b in y
                        select a + b;

            Assert.That(total, Is.EqualTo(Option.None));
        }

        [Test]
        public void SelectMany_None_2()
        {
            Option<int> x = new(10);
            Option<int> y = new(20);
            Option<int> z = Option.None;

            var total = from a in x
                        from b in y
                        from c in z
                        select a + b + c;

            Assert.That(total, Is.EqualTo(Option.None));
        }
    }
}
