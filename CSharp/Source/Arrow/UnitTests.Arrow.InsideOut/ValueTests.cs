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
    public class ValueTests : TestBase
    {
        [Test]
        public void BasicValues()
        {
            Check<BoolValue, bool>(new(){Value = true});
            Check<Int32Value, int>(new(){Value = 234});
            Check<Int64Value, long>(new(){Value = 232344});
            Check<DoubleValue, double>(new(){Value = 50.5});
            Check<DecimalValue, decimal>(new(){Value = 201.1m});
            Check<TimeSpanValue, TimeSpan>(new(){Value = DateTime.Now.TimeOfDay});
            Check<DateTimeValue, DateTime>(new(){Value = DateTime.Now});
            Check<DateTimeValue, DateTime>(new(){Value = DateTime.UtcNow});
            Check<StringValue, string>(new(){Value = null});
            Check<StringValue, string>(new(){Value = ""});
            Check<StringValue, string>(new(){Value = "A"});
            Check<StringValue, string>(new(){Value = "Jack and Sawyer"});
        }

        [Test]
        public void Struct()
        {
            StructValue initial = new StructValue()
            {
                Members =
                {
                    {"Name", Value.From("Jack")},
                    {"Age", Value.From(42)},
                    {"Location", new StructValue()
                                 {
                                    Members = 
                                    {
                                        {"Latitude", Value.From(51.1781m)},
                                        {"Longitude", Value.From(-4.65965m)}
                                    }
                                 }
                    }
                }
            };

            var roundTrip = RoundTrip(initial);
            
            Assert.That(initial.Members.Count, Is.EqualTo(roundTrip.Members.Count));
            Assert.That(((StringValue)(roundTrip.Members["Name"])).Value, Is.EqualTo("Jack"));
            Assert.That(((Int32Value)(roundTrip.Members["Age"])).Value, Is.EqualTo(42));

            var location = (StructValue)(roundTrip.Members["Location"]);
            Assert.That(location.Members.Count, Is.EqualTo(2));
            Assert.That(((DecimalValue)(location.Members["Latitude"])).Value, Is.EqualTo(51.1781m));
            Assert.That(((DecimalValue)(location.Members["Longitude"])).Value, Is.EqualTo(-4.65965m));
        }

        private void Check<TBasic, T>(TBasic value) where TBasic : BasicValue<T>
        {
            var roundTrip = RoundTrip(value);

            Assert.That(roundTrip.Value, Is.EqualTo(value.Value));
            Assert.That(roundTrip.AsObject(), Is.EqualTo(value.AsObject()));
            Assert.That(roundTrip.Type(), Is.EqualTo(value.Type()));

            Value asValue = value;
            var roundTripAsValue = RoundTrip(asValue);
            Assert.That(value.GetType(), Is.EqualTo(roundTripAsValue.GetType()));
        }
    }
}
