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
    public class JsonToObjectTests : JsonTestBase
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
            var toObject = new JsonToObject();

            var data = toObject.Inflate(json);
            Assert.That(data, Is.EqualTo(expected));
        }

        [Test]
        public void Array()
        {
            var toObject = new JsonToObject();

            var data = toObject.Inflate("[5, 10, 15, 20]");
            Assert.That(data, Is.Not.Null);

            var array = data as IList<object>;
            Assert.That(array, Is.Not.Null);

            Assert.That(array.Count, Is.EqualTo(4));
            Assert.That(array[0], Is.EqualTo(5));
            Assert.That(array[1], Is.EqualTo(10));
            Assert.That(array[2], Is.EqualTo(15));
            Assert.That(array[3], Is.EqualTo(20));
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

            var toObject = new JsonToObject();

            var data = toObject.Inflate(json);
            Assert.That(data, Is.Not.Null);

            var @object = data as IDictionary<string, object>;
            Assert.That(@object, Is.Not.Null);

            Assert.That(@object["Name"], Is.EqualTo("Jack"));
            Assert.That(@object["Age"], Is.EqualTo(42));
            Assert.That(@object["Rate"], Is.EqualTo(100.5));
            Assert.That(@object["Enabled"], Is.True);
            Assert.That(@object["Disabled"], Is.False);
            Assert.That(@object["Location"], Is.Null);
        }

        [Test]
        public void ArrayOfIntegers()
        {
            var json = ToJson(@"
                {
                    'Values': [1, 2, 3]
                }
            ");

            var toObject = new JsonToObject();

            var data = toObject.Inflate(json);
            Assert.That(data, Is.Not.Null);

            var @object = data as IDictionary<string, object>;
            Assert.That(@object, Is.Not.Null);

            var array = @object["Values"] as IList<object>;
            Assert.That(array, Is.Not.Null);

            Assert.That(array[0], Is.EqualTo(1));
            Assert.That(array[1], Is.EqualTo(2));
            Assert.That(array[2], Is.EqualTo(3));
        }

        [Test]
        public void ArrayOfMixed()
        {
            var json = ToJson(@"
                {
                    'Values': ['Ben', true, 3]
                }
            ");

            var toObject = new JsonToObject();

            var data = toObject.Inflate(json);
            Assert.That(data, Is.Not.Null);

            var @object = data as IDictionary<string, object>;
            Assert.That(@object, Is.Not.Null);

            var array = @object["Values"] as IList<object>;
            Assert.That(array, Is.Not.Null);

            Assert.That(array[0], Is.EqualTo("Ben"));
            Assert.That(array[1], Is.True);
            Assert.That(array[2], Is.EqualTo(3));
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

            var toObject = new JsonToObject();
            
            var data = toObject.Inflate(json);
            Assert.That(data, Is.Not.Null);

            var root = (IDictionary<string, object>)data;
            Assert.That(root["Name"], Is.EqualTo("Kate"));
            
            var location = (IDictionary<string, object>)root["Location"];
            Assert.That(location["Name"], Is.EqualTo("The Island"));
        }
    }
}
