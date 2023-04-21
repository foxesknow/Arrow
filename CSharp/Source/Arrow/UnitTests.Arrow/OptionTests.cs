using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow;

using NUnit.Framework;

namespace UnitTests.Arrow
{
    [TestFixture]
    public class OptionTests
    {
        [Test]
        public void Initialization_Default()
        {
            Option<int> option = default;
            Assert.That(option.IsSome, Is.False);
            Assert.That(option.IsNone, Is.True);
        }

        [Test]
        public void Initialization_FromNone()
        {
            Option<int> option = Option.None;
            Assert.That(option.IsSome, Is.False);
            Assert.That(option.IsNone, Is.True);
        }

        [Test]
        public void Initialization_FromValue()
        {
            Option<int> option = new(10);
            Assert.That(option.IsSome, Is.True);
            Assert.That(option.IsNone, Is.False);
        }

        [Test]
        public void Initialization_ForceNull()
        {
            Option<string> option = new(null!);
            Assert.That(option.IsSome, Is.False);
            Assert.That(option.IsNone, Is.True);
        }

        [Test]
        public void Implicit_Reference()
        {
            Option<string> name = null;
            Assert.That(name, Is.EqualTo(Option.None));
        }

        [Test]
        public void Implicit_FromOption()
        {
            Option<int> age = 10;
            Option<int> age2 = age;

            Assert.That(age.Value(), Is.EqualTo(age2.Value()));
        }

        [Test]
        public void Implicit_Value()
        {
            Option<int> age = 40;    
            Assert.That(age.Value(), Is.EqualTo(40));
        }

        [Test]
        public void Match_Some()
        {
            Option<string> name = "Hello";

            var length = name.Match
            (
                some: value => value.Length,
                none: () => -1
            );

            Assert.That(length, Is.EqualTo(5));
        }

        [Test]
        public void Match_None()
        {
            Option<string> name = Option.None;

            var length = name.Match
            (
                some: value => value.Length,
                none: () => -1
            );

            Assert.That(length, Is.EqualTo(-1));
        }

        [Test]
        public void Match_Some_State()
        {
            int delta = 1;
            Option<string> name = "Hello";

            var length = name.Match
            (
                delta,
                some: static (value, delta) => value.Length + delta,
                none: static delta => delta
            );

            Assert.That(length, Is.EqualTo(6));
        }

        [Test]
        public void Match_None_State()
        {
            int delta = 1;
            Option<string> name = Option.None;

            var length = name.Match
            (
                delta,
                some: static (value, state) => value.Length + state,
                none: static state => state
            );

            Assert.That(length, Is.EqualTo(1));
        }

        [Test]
        public void Select_Some()
        {
            Option<int> x = 80;
            var y = x.Select(static y => y * 2);
            Assert.That(y, Is.Not.EqualTo(Option.None));
            Assert.That(y.Value(), Is.EqualTo(160));
        }

        [Test]
        public void Select_None()
        {
            Option<int> x = Option.None;
            var y = x.Select(static y => y * 2);
            Assert.That(y, Is.EqualTo(Option.None));
        }

        [Test]
        public void Select_Some_State()
        {
            var multiplier = 2;
            Option<int> x = 80;

            var y = x.Select(multiplier, static (y, state) => y * state);
            Assert.That(y, Is.Not.EqualTo(Option.None));
            Assert.That(y.Value(), Is.EqualTo(160));
        }

        [Test]
        public void Select_None_State()
        {
            var multiplier = 2;
            Option<int> x = Option.None;
            
            var y = x.Select(multiplier, static (y, state) => y * state);
            Assert.That(y, Is.EqualTo(Option.None));
        }

        [Test]
        public void Bind()
        {
            Option<string> name = "Robert";
            var nameLength = name.Bind(y => GetLength(y));
            Assert.That(nameLength.Value(), Is.EqualTo(6));

            Option<string> address = default;
            var addressLength = address.Bind(y => GetLength(y));
            Assert.That(addressLength, Is.EqualTo(Option.None));            
        }

        [Test]
        public void Bind_State()
        {
            var surname= "Smith";

            Option<string> name = "Robert";
            var nameLength = name.Bind(surname, (y, state) => GetLength(y + state));
            Assert.That(nameLength.Value(), Is.EqualTo(11));

            Option<string> address = default;
            var addressLength = address.Bind(surname, (y, state) => GetLength(y + state));
            Assert.That(addressLength, Is.EqualTo(Option.None));            
        }

        [Test]
        public void TryGetValue_Some()
        {
            Option<int> x = 80;
            Assert.That(x.TryGetValue(out var value), Is.True);
            Assert.That(value, Is.EqualTo(80));
        }

        [Test]
        public void TryGetValue_Some_Nullable()
        {
            Option<string> x = "Hello";

            if(x.TryGetValue(out var value))
            {
                Assert.That(value, Is.EqualTo("Hello"));
            }
            else
            {
                Assert.Fail("expected a value");
            }            
        }

        [Test]
        public void TryGetValue_None_Nullable()
        {
            Option<string> x = default;

            if(x.TryGetValue(out var value))
            {
                Assert.Fail("expected a value");                
            }
            else
            {
                Assert.That(value, Is.Null);
            }            
        }

        [Test]
        public void Value_Some()
        {
            Option<int> x = 80;
            Assert.DoesNotThrow(() => x.Value());
            Assert.That(x.Value(), Is.EqualTo(80));
        }

        [Test]
        public void Value_None()
        {
            Option<int> x = default;
            Assert.Catch(() => x.Value());
        }

        [Test]
        public void ValueOr_Some()
        {
            Option<int> x = 80;
            Assert.That(x.ValueOr(1), Is.EqualTo(80));
        }

        [Test]
        public void ValueOr_None()
        {
            Option<int> x = Option.None;
            Assert.That(x.ValueOr(1), Is.EqualTo(1));
        }

        [Test]
        public void ValueOr_Func_Some()
        {
            bool called = false;

            Func<int> factory = () =>
            {
                called = true;
                return 999;
            };

            Option<int> x = 80;
            Assert.That(x.ValueOr(factory), Is.EqualTo(80));
            Assert.That(called, Is.False);
        }

        [Test]
        public void ValueOr_Func_None()
        {
            bool called = false;

            Func<int> factory = () =>
            {
                called = true;
                return 999;
            };

            Option<int> x = Option.None;
            Assert.That(x.ValueOr(factory), Is.EqualTo(999));
            Assert.That(called, Is.True);
        }

        [Test]
        public void ValueOr_Func_None_State()
        {
            var defaultValue = 999;
            bool called = false;

            Func<int, int> factory = state =>
            {
                called = true;
                return state;
            };

            Option<int> x = Option.None;
            Assert.That(x.ValueOr(defaultValue, factory), Is.EqualTo(999));
            Assert.That(called, Is.True);
        }

        [Test]
        public void Equality()
        {
            Option<string> name = default;
            Assert.That(name.Equals(Option.None), Is.True);
            Assert.That(name.Equals((object)Option.None), Is.True);
            Assert.That(Option.None.Equals(name), Is.True);

            Assert.That(Option.None.Equals((object)name), Is.True);
            Assert.That(name, Is.EqualTo(Option.None));
            Assert.That(Option.None, Is.EqualTo(name));

            Option<string> name2 = default;
            Assert.That(name.Equals(name2), Is.True);
            Assert.That(name, Is.EqualTo(name2));
        }

        [Test]
        public void Operator_Equals()
        {
            Option<string> name = default;
            Assert.That(name == Option.None, Is.True);
            Assert.That(Option.None == name, Is.True);

            Option<int> x = new(10);
            Option<int> y = new(10);
            Assert.That(x == y, Is.True);
            Assert.That(y == x, Is.True);

            Option<int> z = new(20);
            Assert.That(x == z, Is.False);
            Assert.That(z == x, Is.False);
        }

        [Test]
        public void Operator_NotEquals()
        {
            Option<string> name = default;
            Assert.That(name != Option.None, Is.False);
            Assert.That(Option.None != name, Is.False);

            Option<int> x = new(10);
            Option<int> y = new(10);
            Assert.That(x != y, Is.False);
            Assert.That(y != x, Is.False);

            Option<int> z = new(20);
            Assert.That(x != z, Is.True);
            Assert.That(z != x, Is.True);
        }

        [Test]
        public void Hashing_Some()
        {
            Option<int> x = new(10);
            Option<int> y = new(10);
            Assert.That(x.GetHashCode(), Is.EqualTo(y.GetHashCode()));
        }

        [Test]
        public void Hashing_None()
        {
            Option<int> x = default;
            Assert.That(x.GetHashCode(), Is.EqualTo(0));
            Assert.That(x.GetHashCode(), Is.EqualTo(Option.None.GetHashCode()));

            Option<int> y = default;
            Assert.That(x.GetHashCode(), Is.EqualTo(y.GetHashCode()));
        }

        [Test]
        public void AsString_Some()
        {
            Option<string> name = new("Hello");
            Assert.That(name.ToString(), Is.EqualTo("Hello"));
        }

        [Test]
        public void AsString_None()
        {
            Option<string> name = default;
            Assert.That(name.ToString(), Is.EqualTo("none"));
        }

        [Test]
        public void From_Value_HasValue()
        {
            int? id = 10;
            var value = Option.From(id);
            Assert.That(value.ValueOr(-1), Is.EqualTo(10));
        }

        [Test]
        public void From_Value_IsNull()
        {
            int? id = null;
            var value = Option.From(id);
            Assert.That(value.ValueOr(-1), Is.EqualTo(-1));
        }

        [Test]
        public void From_Reference_HasValue()
        {
            var name = "Bob";
            var value = Option.From(name);
            Assert.That(value.ValueOr("Fred"), Is.EqualTo("Bob"));
        }

        [Test]
        public void From_Reference_IsNull()
        {
            string? name = null;
            var value = Option.From(name);
            Assert.That(value.ValueOr("Fred"), Is.EqualTo("Fred"));
        }

        [Test]
        [TestCase(0)]
        [TestCase(0)]
        [TestCase(2)]
        public void LeftIdentify(int a)
        {
            // Taken from https://blog.ploeh.dk/2022/04/25/the-maybe-monad/

            Func<int, Option<int>> @return = i => new Option<int>(i);
            Func<int, Option<double>> h = i => i != 0 ? new Option<double>(1.0 / i) : Option.None;
 
            Assert.That(@return(a).Bind(h), Is.EqualTo(h(a)));
        }

        [Test]
        [TestCase("")]
        [TestCase("foo")]
        [TestCase("42")]
        [TestCase("1337")]
        public void RightIdentity(string a)
        {
            // Taken from https://blog.ploeh.dk/2022/04/25/the-maybe-monad/

            static Option<int> f(string s)
            {
                if(int.TryParse(s, out var i))
                    return new(i);
                else
                    return Option.None;
            }
            Func<int, Option<int>> @return = i => new Option<int>(i);
 
            Option<int> m = f(a);
 
            Assert.That(m.Bind(@return), Is.EqualTo(m));
        }

        [Theory]
        [TestCase("bar")]
        [TestCase("-1")]
        [TestCase("0")]
        [TestCase("4")]
        public void Associativity(string a)
        {
            // Taken from https://blog.ploeh.dk/2022/04/25/the-maybe-monad/

            static Option<double> Sqrt(double d)
            {
                var result = Math.Sqrt(d);
                switch (result)
                {
                    case double.NaN:
                    case double.PositiveInfinity: 
                        return Option.None;
                    
                    default: 
                        return new Option<double>(result);
                }
            }

            Option<int> f(string s)
            {
                if(int.TryParse(s, out var i))
                    return new Option<int>(i);
                else
                    return Option.None;
            }
            Func<int, Option<double>> g = i => Sqrt(i);
            Func<double, Option<double>> h = d => d == 0 ? Option.None : new Option<double>(1 / d);
 
            var m = f(a); 
            Assert.That(m.Bind(g).Bind(h), Is.EqualTo(m.Bind(x => g(x).Bind(h))));
        }

        private static Option<int> GetLength(string? value)
        {
            if(value is null) return Option.None;

            return value.Length;
        }
    }
}
