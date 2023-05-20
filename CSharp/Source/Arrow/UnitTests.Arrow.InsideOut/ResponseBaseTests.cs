using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.InsideOut;
using Arrow.InsideOut.Transport;

using NUnit.Framework;

namespace UnitTests.Arrow.InsideOut
{
    [TestFixture]
    public class ResponseBaseTests : TestBase
    {
        [Test]
        public void ExceptionResponse_RoundTrip()
        {
            var response = new ExceptionResponse("oops!")
            {
                ExceptionType = nameof(InsideOutException),
            };

            Check(response);
        }

        [Test]
        public void ExecuteResponse_RoundTrip()
        {
            var response = new ExecuteResponse()
            {
                Success = true,
                Message = "We did it!"
            };

            Check(response);
        }

        [Test]
        public void Details_RoundTrip()
        {
            var response = new NodeDetails()
            {
                Commands = 
                {
                    new Command("Add")
                    {
                        Description = "Adds 2 numbers",
                        Parameters =
                        {
                            new DoubleParameter("lhs"),
                            new DoubleParameter("rhs"),
                        }
                    }
                },
                Values =
                {
                    {"Last", Value.From(10.0)},
                    {"Calculations Done", Value.From(5)},
                    {"Model", Value.From("ZX81")}
                }
            };

            Check(response);
        }

        private void Check<T>(T value) where T : ResponseBase
        {
            var roundTrip = RoundTrip(value);
            Assert.That(roundTrip.Type(), Is.EqualTo(value.Type()));

            Value asValue = value;
            var roundTripAsValue = RoundTrip(asValue);
            Assert.That(value.GetType(), Is.EqualTo(roundTripAsValue.GetType()));
        }
    }
}
