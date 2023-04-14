using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Arrow.Text.Json.Serialization;

using NUnit.Framework;

namespace UnitTests.Arrow.Text.Json.Serialization
{
    [TestFixture]
    public class JsonAsRawTextConverterTests
    {
        [Test]
        public void ValueIsNull()
        {
            var options = MakeOptions();
            var json = ToJson("{'Name': 'Jack', 'Value': null}");

            var data = JsonSerializer.Deserialize<TestData>(json, options);
            Assert.That(data, Is.Not.Null);
            Assert.That(data.Name, Is.EqualTo("Jack"));
            Assert.That(data.Value, Is.Null);

            var roundTrip = JsonSerializer.Serialize<TestData>(data, options);
            Assert.That(roundTrip, Has.Length.GreaterThan(0));
        }

        [Test]
        public void ValueIsString()
        {
            var options = MakeOptions();
            var json = ToJson("{'Name': 'Jack', 'Value': 'The Island'}");

            var data = JsonSerializer.Deserialize<TestData>(json, options);
            Assert.That(data, Is.Not.Null);
            Assert.That(data.Name, Is.EqualTo("Jack"));
            Assert.That(data.Value, Is.EqualTo("\"The Island\""));
        }

        [Test]
        public void ValueIsArray()
        {
            var options = MakeOptions();
            var json = ToJson("{'Name': 'Jack', 'Value': [1, 2, 3]}");

            var data = JsonSerializer.Deserialize<TestData>(json, options);
            Assert.That(data, Is.Not.Null);
            Assert.That(data.Name, Is.EqualTo("Jack"));
            Assert.That(data.Value, Is.EqualTo("[1, 2, 3]"));

            var roundTrip = JsonSerializer.Serialize<TestData>(data, options);
            Assert.That(roundTrip, Has.Length.GreaterThan(0));
        }

        [Test]
        public void ValueIsObject()
        {
            var options = MakeOptions();
            var json = ToJson("{'Name': 'Jack', 'Value': {'Age' : 42}}");

            var data = JsonSerializer.Deserialize<TestData>(json, options);
            Assert.That(data, Is.Not.Null);
            Assert.That(data.Name, Is.EqualTo("Jack"));
            Assert.That(data.Value, Is.EqualTo("{\"Age\" : 42}"));
        }

        private string ToJson(string value)
        {
            return value.Replace('\'', '"');
        }

        private JsonSerializerOptions MakeOptions()
        {
            return new()
            {
                PropertyNameCaseInsensitive = true
            };
        }

        class TestData
        {
            public string Name{get; set;}

            [JsonConverter(typeof(JsonAsRawTextConverter))]
            public string Value{get; set;}
        }
    }
}
