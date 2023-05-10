using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.InsideOut.Transport;

using NUnit.Framework;

namespace UnitTests.Arrow.InsideOut
{
    public abstract class TestBase
    {
        protected T RoundTrip<T>(T @object) where T : class
        {
            var serializer = new InsideOutJsonSerializer();
            var json = serializer.Serialize(@object);
            Assert.That(json, Is.Not.Null & Has.Length.GreaterThan(0));
            Assert.That(json, Is.Not.Null & Does.Not.Contain("`"));

            var inflated = serializer.Deserialize<T>(json);
            Assert.That(inflated, Is.Not.Null);

            return inflated!;
        }
    }
}
