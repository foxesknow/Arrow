using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.InsideOut;
using Arrow.InsideOut.Transport;

using NUnit.Framework;


namespace UnitTests.Arrow.InsideOut.Transport
{
    [TestFixture]
    public class TransportRequestTests : TransportTestsBase
    {
        [Test]
        public void Serialize_GetDetails()
        {
            var request = new TransportRequest(NodeFunction.GetDetails, m_PublisherID, m_RequestID)
            {
                Request = null
            };

            var roundTrip = RoundTrip(request);
            Assert.That(request.NodeFunction, Is.EqualTo(roundTrip.NodeFunction));
            Assert.That(request.PublisherID, Is.EqualTo(roundTrip.PublisherID));
            Assert.That(request.RequestID, Is.EqualTo(roundTrip.RequestID));
            Assert.That(request.Request, Is.EqualTo(roundTrip.Request));
        }

        [Test]
        public void Serialize_Execute()
        {
            var request = new TransportRequest(NodeFunction.Execute, m_PublisherID, m_RequestID)
            {
                Request = new ExecuteRequest("some/thing")
                {
                    Arguments =
                    {
                        P_Bool.MakeArgument(true),
                        P_Int32.MakeArgument(10),
                        P_Int64.MakeArgument(20),
                        P_Double.MakeArgument(40),
                        P_Decimal.MakeArgument(80),
                        P_TimeSpan.MakeArgument(new(10, 20, 30, 40)),
                        P_DateTime.MakeArgument(DateTime.Now),
                        P_String.MakeArgument("hello"),
                        P_Suggestion.MakeArgument("foo"),
                        P_SingleItem.MakeArgumentFromObject(2),
                        P_MultiItems.MakeArgument(P_MultiItems.Items)
                    }
                }
            };

            var roundTrip = RoundTrip(request);
            Assert.That(request.NodeFunction, Is.EqualTo(roundTrip.NodeFunction));
            Assert.That(request.RequestID, Is.EqualTo(roundTrip.RequestID));
            Assert.That(request.PublisherID, Is.EqualTo(roundTrip.PublisherID));

            var inRequest = (ExecuteRequest)request.Request;
            var outRequest = (ExecuteRequest)roundTrip.Request;

            Assert.That(inRequest.Arguments.Count, Is.EqualTo(outRequest.Arguments.Count));

            for(var i = 0; i < 9; i++)
            {
                var lhs = inRequest.Arguments[i];
                var rhs = outRequest.Arguments[i];

                Assert.That(lhs.GetType(), Is.EqualTo(rhs.GetType()));
                Assert.That(lhs.Name, Is.EqualTo(rhs.Name));
                Assert.That(lhs.AsObject(), Is.EqualTo(rhs.AsObject()));
            }

            var single = (SingleItemArgument)outRequest.Arguments[9];
            Assert.That(single.Value.Name, Is.EqualTo("Y"));
            Assert.That(single.Value.Ordinal, Is.EqualTo(2));

            var multi = (MultipleItemsArgument)outRequest.Arguments[10];
            Assert.That(multi.Value.Count, Is.EqualTo(3));
            Assert.That(multi.Value[0].Name, Is.EqualTo("X"));
            Assert.That(multi.Value[0].Ordinal, Is.EqualTo(1));
            Assert.That(multi.Value[1].Name, Is.EqualTo("Y"));
            Assert.That(multi.Value[1].Ordinal, Is.EqualTo(2));
            Assert.That(multi.Value[2].Name, Is.EqualTo("Z"));
            Assert.That(multi.Value[2].Ordinal, Is.EqualTo(4));
        }
    }
}
