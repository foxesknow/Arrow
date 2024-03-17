using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tango.Workbench.Data;

using NUnit.Framework;

namespace UnitTests.Tango.Workbench.Data
{
    [TestFixture]
    public class StructuredObjectTests
    {
        [Test]
        public void Initialization()
        {
            Assert.DoesNotThrow(() => new StructuredObject());
        }

        [Test]
        public void Add()
        {
            var so = new StructuredObject();
            Assert.That(so.Count, Is.Zero);

            so.Add("Name", "Jack");
            Assert.That(so.Count, Is.EqualTo(1));

            Assert.That(so["name"], Is.EqualTo("Jack"));
            Assert.That(so["Name"], Is.EqualTo("Jack"));
            Assert.That(so[0], Is.EqualTo("Jack"));
        }

        [Test]
        public void ItemNotPresent()
        {
            var so = new StructuredObject();
            
            object temp = null;
            Assert.Catch(() => temp = so["name"]);
            Assert.Catch(() => temp = so[0]);
        }

        [Test]
        public void TryGetValue()
        {
            var so = new StructuredObject();
            Assert.That(so.TryGetValue("name", out var name), Is.False);

            so.Add("Name", "Ben");
            Assert.That(so.TryGetValue("name", out name), Is.True);
            Assert.That(name, Is.EqualTo("Ben"));
        }

        [Test]
        public void MakeExpandoObject()
        {
            var so = new StructuredObject()
            {
                {"Name", "Jack"},
                {"Age", 42},
            };

            var expando = so.MakeExpandoObject();
            Assert.That(expando, Is.Not.Null);

            dynamic d = expando;
            Assert.That(d.Name, Is.EqualTo("Jack"));
            Assert.That(d.Age, Is.EqualTo(42));
        }

        [Test]
        public void From_Value()
        {
            var so = StructuredObject.From(42);
            Assert.That(so.Count, Is.EqualTo(1));
            Assert.That(so["Value"], Is.EqualTo(42));
        }

        [Test]
        public void From_KeyValuePair()
        {
            var so = StructuredObject.From(42);
            so.Add("Name", "Ben");
            
            var copy = StructuredObject.From(so);
            Assert.That(copy.Count, Is.EqualTo(2));
            Assert.That(copy["Value"], Is.EqualTo(42));
            Assert.That(copy["Name"], Is.EqualTo("Ben"));
        }

        [Test]
        public void From_Object()
        {
            var anon = new
            {
                Name = "Jack",
                Age = 42,
                Location = "Island"
            };

            var so = StructuredObject.From(anon);
            Assert.That(so.Count, Is.EqualTo(3));
            Assert.That(so["Name"], Is.EqualTo("Jack"));
            Assert.That(so["Age"], Is.EqualTo(42));
            Assert.That(so["Location"], Is.EqualTo("Island"));
        }
    }
}
