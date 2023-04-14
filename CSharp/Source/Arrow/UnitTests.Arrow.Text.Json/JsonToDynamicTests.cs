using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Text.Json;

using NUnit.Framework;

namespace UnitTests.Arrow.Text.Json
{
    [TestFixture]
    public class JsonToDynamicTests
    {
        [Test]
        [TestCase("1", 1)]
        [TestCase("100.5", 100.5)]
        [TestCase("true", true)]
        [TestCase("false", false)]
        [TestCase("null", null)]
        [TestCase("\"Hello\"", "Hello")]
        public void PlainValues(string json, object expected)
        {
            var data = JsonToDynamic.Inflate(json);
            Assert.That(data, Is.EqualTo(expected));
        }

        [Test]
        public void Array()
        {
            var data = JsonToDynamic.Inflate("[5, 10, 15, 20]");
            Assert.That(data.Count, Is.EqualTo(4));
            Assert.That(data[0], Is.EqualTo(5));
            Assert.That(data[1], Is.EqualTo(10));
            Assert.That(data[2], Is.EqualTo(15));
            Assert.That(data[3], Is.EqualTo(20));
        }

        [Test]
        public void BasicTypes()
        {
            var json = ToJson(@"
                {
                    'Name' : 'Jack',
                    'Age' : 42,
                    'Rate' : 100.5,
                    'Enabled' : true,
                    'Disabled' : false,
                    'Location' : null
                }
            ");

            var data = JsonToDynamic.Inflate(json);
            Assert.That(data, Is.Not.Null);

            Assert.That(data.Name, Is.EqualTo("Jack"));
            Assert.That(data.Age, Is.EqualTo(42));
            Assert.That(data.Rate, Is.EqualTo(100.5));
            Assert.That(data.Enabled, Is.True);
            Assert.That(data.Disabled, Is.False);
            Assert.That(data.Location, Is.Null);
        }

        [Test]
        public void ArrayOfIntegers()
        {
            var json = ToJson(@"
                {
                    'Values': [1, 2, 3]
                }
            ");

            var data = JsonToDynamic.Inflate(json);
            Assert.That(data, Is.Not.Null);

            Assert.That(data.Values[0], Is.EqualTo(1));
            Assert.That(data.Values[1], Is.EqualTo(2));
            Assert.That(data.Values[2], Is.EqualTo(3));
        }

        [Test]
        public void ArrayOfMixed()
        {
            var json = ToJson(@"
                {
                    'Values': ['Ben', true, 3]
                }
            ");

            var data = JsonToDynamic.Inflate(json);
            Assert.That(data, Is.Not.Null);

            Assert.That(data.Values[0], Is.EqualTo("Ben"));
            Assert.That(data.Values[1], Is.True);
            Assert.That(data.Values[2], Is.EqualTo(3));
        }

        [Test]
        public void NestedObjects()
        {
            var json = ToJson(@"
                {
                    'Name' : 'Kate',
                    'Location' : {
                        'Name': 'The Island'
                    }
                }
            ");

            var data = JsonToDynamic.Inflate(json);
            Assert.That(data, Is.Not.Null);

            Assert.That(data.Name, Is.EqualTo("Kate"));
            Assert.That(data.Location.Name, Is.EqualTo("The Island"));
        }

        private string ToJson(string value)
        {
            return value.Replace('\'', '"');
        }
    }
}
