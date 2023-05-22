using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.InsideOut.Transport;

using NUnit.Framework;

namespace UnitTests.Arrow.InsideOut.Transport
{
    [TestFixture]
    public class RequestIDTests
    {
        [Test]
        public void Initialization()
        {
            var applicationID = Guid.NewGuid();
            var correlationID = Guid.NewGuid();

            var requestID = new RequestID(applicationID, correlationID);
            Assert.That(requestID.ApplicationID, Is.EqualTo(applicationID));
            Assert.That(requestID.CorrelationID, Is.EqualTo(correlationID));

            Assert.That(requestID.ToString(), Is.Not.Null & Has.Length.GreaterThan(0));
        }

        [Test]
        public void Equality()
        {
            var applicationID = Guid.NewGuid();
            var correlationID = Guid.NewGuid();

            var requestID = new RequestID(applicationID, correlationID);
            Assert.That(requestID, Is.EqualTo(requestID));
        }

        [Test]
        public void Inequality()
        {
            var r1 = new RequestID(Guid.NewGuid(), Guid.NewGuid());
            var r2 = new RequestID(Guid.NewGuid(), Guid.NewGuid());
            Assert.That(r1, Is.Not.EqualTo(r2));
        }
    }
}
