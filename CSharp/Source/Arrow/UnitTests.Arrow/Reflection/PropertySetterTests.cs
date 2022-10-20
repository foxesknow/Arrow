using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Reflection;

using NUnit.Framework;

namespace UnitTests.Arrow.Reflection
{
    [TestFixture]
    public class PropertySetterTests
    {
        [Test]
        public void Set_InvalidValues()
        {
            Assert.Catch(() => PropertySetter.Make(new TestClass()).Set((string)null, "Jack"));
            Assert.Catch(() => PropertySetter.Make(new TestClass()).Set("", "Jack"));
            Assert.Catch(() => PropertySetter.Make(new TestClass()).Set("   ", "Jack"));
        }

        [Test]
        public void Set_NoSuchProperty()
        {
            Assert.Catch(() => PropertySetter.Make(new TestClass()).Set("Foobar", "Jack"));
        }

        [Test]
        public void Set_PropertyIsPrivate()
        {
            Assert.Catch(() => PropertySetter.Make(new TestClass()).Set("Location", "Island"));
        }

        [Test]
        public void Set_PropertyIsProtected()
        {
            Assert.Catch(() => PropertySetter.Make(new TestClass()).Set("Partner", "Kate"));
        }

        [Test]
        public void Set_PropertyIsReadOnly()
        {
            Assert.Catch(() => PropertySetter.Make(new TestClass()).Set("Classification", 2));
        }

        [Test]
        public void Set()
        {
            var obj = PropertySetter.Make(new TestClass())
                                    .Set("Name", "Jack")
                                    .Object;

            Assert.That(obj, Is.Not.Null);
            Assert.That(obj.Name, Is.EqualTo("Jack"));
        }

        [Test]
        public void Set_Multiple()
        {
            var obj = PropertySetter.Make(new TestClass())
                                    .Set("Name", "Jack")
                                    .Set("Age", 36)
                                    .Set("HourlyRate", 41.5m)
                                    .Object;

            Assert.That(obj, Is.Not.Null);
            Assert.That(obj.Name, Is.EqualTo("Jack"));
            Assert.That(obj.Age, Is.EqualTo(36));
            Assert.That(obj.HourlyRate, Is.EqualTo(41.5m));
        }

        [Test]
        public void Set_Multiple_Expression()
        {
            var obj = PropertySetter.Make(new TestClass())
                                    .Set(o => o.Name, "Jack")
                                    .Set(o => o.Age, 36)
                                    .Set(o => o.HourlyRate, 41.5m)
                                    .Object;

            Assert.That(obj, Is.Not.Null);
            Assert.That(obj.Name, Is.EqualTo("Jack"));
            Assert.That(obj.Age, Is.EqualTo(36));
            Assert.That(obj.HourlyRate, Is.EqualTo(41.5m));
        }

        [Test]
        public void Set_Conversion()
        {
            var obj = PropertySetter.Make(new TestClass())
                                    .Set("HourlyRate", 20)
                                    .Object;

            Assert.That(obj, Is.Not.Null);
            Assert.That(obj.HourlyRate, Is.EqualTo(20m));
        }

        [Test]
        public void Set_Conversion_String_To_Integer()
        {
            var obj = PropertySetter.Make(new TestClass())
                                    .Set("Age", "10")
                                    .Object;

            Assert.That(obj, Is.Not.Null);
            Assert.That(obj.Age, Is.EqualTo(10));
        }

        [Test]
        public void Set_Null()
        {
            var before = new TestClass("Ben");
            Assert.That(before.Name, Is.EqualTo("Ben"));

            var obj = PropertySetter.Make(before)
                                    .Set("Name", null)
                                    .Object;

            Assert.That(obj, Is.Not.Null);
            Assert.That(obj.Name, Is.Null);
        }

        [Test]
        public void Set_InitOnly()
        {
            var obj = PropertySetter.Make(new InitOnly())
                                    .Set("Name", "Jack")
                                    .Set("Age", 39)
                                    .Object;

            Assert.That(obj, Is.Not.Null);
            Assert.That(obj.Name, Is.EqualTo("Jack"));
            Assert.That(obj.Age, Is.EqualTo(39));
        }

        [Test]
        public void Set_InitOnly_Expression()
        {
            var obj = PropertySetter.Make(new InitOnly())
                                    .Set(o => o.Name, "Jack")
                                    .Set(o => o.Age, 39)
                                    .Object;

            Assert.That(obj, Is.Not.Null);
            Assert.That(obj.Name, Is.EqualTo("Jack"));
            Assert.That(obj.Age, Is.EqualTo(39));
        }

        class TestClass
        {
            public TestClass()
            {
            }

            public TestClass(string name)
            {
                this.Name = name;
            }

            public string Name{get; private set;}
            public int Age{get; private set;}
            public decimal HourlyRate{get; private set;}

            private string Location{get; set;}
            protected string Partner{get; private set;}

            public int Classification{get;} = 100;

            public int SomeField = 10;
        }

        class InitOnly
        {
            public string Name{get; init;}
            public int Age{get; init;}
        }
    }
}
