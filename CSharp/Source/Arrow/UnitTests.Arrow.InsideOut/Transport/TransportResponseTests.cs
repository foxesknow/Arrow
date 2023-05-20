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
    /// <summary>
    /// System.Text.Json isn't as flexible as Newtonsoft, so we have to add attributes to get things to serialize.
    /// It's also not as flexible with properties that are collections. It wants to assign a collection to the property
    /// rather than call the getter and use the returned collection.
    /// </summary>
    [TestFixture]
    public class TransportResponseTests : TransportTestsBase
    {
        [Test]
        public void Serialize_GetDetails()
        {
            var command = new Command("Test")
            {
                Description = "not a lot",
                Parameters = {P_Bool, P_Int32, P_Int64, P_Double, P_Decimal, P_TimeSpan, P_DateTime, P_String, P_Suggestion, P_SingleItem, P_MultiItems}
            };
            
            var now = DateTime.Now;
            var response = new TransportResponse(NodeResponse.GetDetails, m_RequestID, new NodeDetails()
            {
                Commands = {command},
                Values =
                {
                    {"a", Value.From(true)},
                    {"b", Value.From(1)},
                    {"c", Value.From(2L)},
                    {"d", Value.From(4d)},
                    {"e", Value.From(8m)},
                    {"f", Value.From(now.TimeOfDay)},
                    {"g", Value.From(now)},
                    {"h", Value.From("hello")},
                }
            });

            var roundTrip = RoundTrip(response);
            Assert.That(response.NodeResponse, Is.EqualTo(roundTrip.NodeResponse));
            Assert.That(response.RequestID, Is.EqualTo(roundTrip.RequestID));            
            
            var inDetails = (NodeDetails)response.Response;
            var outDetails = (NodeDetails)roundTrip.Response;

            Assert.That(inDetails.Values.Count, Is.EqualTo(outDetails.Values.Count));
            foreach(var (name, value) in inDetails.Values)
            {
                var lhs = (BasicValue)value;
                var rhs = (BasicValue)(outDetails.Values[name]);

                Assert.That(lhs.Type(), Is.EqualTo(rhs.Type()));
                Assert.That(lhs.AsObject(), Is.EqualTo(rhs.AsObject()));
            }

            Assert.That(outDetails.Commands.Count, Is.EqualTo(1));
            Assert.That(inDetails.Commands.Count, Is.EqualTo(outDetails.Commands.Count));
            
            for(var i = 0; i < 8; i++)
            {
                var lhs = inDetails.Commands[0].Parameters[i];
                var rhs = outDetails.Commands[0].Parameters[i];

                Assert.That(lhs.GetType(), Is.EqualTo(rhs.GetType()));
                Assert.That(lhs.Name, Is.EqualTo(rhs.Name));
                Assert.That(lhs.DefaultAsObject(), Is.EqualTo(rhs.DefaultAsObject()));
            }

            var suggestion = (SuggestionParameter)outDetails.Commands[0].Parameters[8];
            Assert.That(suggestion.Suggestions.Count, Is.EqualTo(3));
            Assert.That(suggestion.Suggestions[0], Is.EqualTo("X"));
            Assert.That(suggestion.Suggestions[1], Is.EqualTo("Y"));
            Assert.That(suggestion.Suggestions[2], Is.EqualTo("Z"));

            var single = (SingleItemParameter)outDetails.Commands[0].Parameters[9];
            Assert.That(single.Items.Count, Is.EqualTo(3));
            Assert.That(single.Items[0].Name, Is.EqualTo("X"));
            Assert.That(single.Items[0].Ordinal, Is.EqualTo(1));
            Assert.That(single.Items[1].Name, Is.EqualTo("Y"));
            Assert.That(single.Items[1].Ordinal, Is.EqualTo(2));
            Assert.That(single.Items[2].Name, Is.EqualTo("Z"));
            Assert.That(single.Items[2].Ordinal, Is.EqualTo(4));

            var multi = (MultipleItemsParameter)outDetails.Commands[0].Parameters[10];
            Assert.That(multi.Items.Count, Is.EqualTo(3));
            Assert.That(multi.Items[0].Name, Is.EqualTo("X"));
            Assert.That(multi.Items[0].Ordinal, Is.EqualTo(1));
            Assert.That(multi.Items[1].Name, Is.EqualTo("Y"));
            Assert.That(multi.Items[1].Ordinal, Is.EqualTo(2));
            Assert.That(multi.Items[2].Name, Is.EqualTo("Z"));
            Assert.That(multi.Items[2].Ordinal, Is.EqualTo(4));
        }

        [Test]
        public void Serialize_Execute()
        {
            var response = new TransportResponse(NodeResponse.Execute, m_RequestID, new ExecuteResponse()
            {
                Success = true,
                Message = "finished",
                Result = Value.From("p99")
            });

            var inResponse = (ExecuteResponse)response.Response;

            var roundTrip = RoundTrip(response);
            Assert.That(response.NodeResponse, Is.EqualTo(roundTrip.NodeResponse));
            Assert.That(response.RequestID, Is.EqualTo(roundTrip.RequestID));
            
            var outResponse = (ExecuteResponse)roundTrip.Response;
            Assert.That(inResponse.Success, Is.EqualTo(outResponse.Success));
            Assert.That(inResponse.Message, Is.EqualTo(outResponse.Message));
            Assert.That(inResponse.Result, Is.EqualTo(outResponse.Result));
        }

        [Test]
        public void Serialize_Exception()
        {
            var response = new TransportResponse(NodeResponse.Exception, m_RequestID, new ExceptionResponse("failed")
            {
                ExceptionType = nameof(ArgumentNullException)
            });

            var inResponse = (ExceptionResponse)response.Response;

            var roundTrip = RoundTrip(response);
            Assert.That(response.NodeResponse, Is.EqualTo(roundTrip.NodeResponse));
            Assert.That(response.RequestID, Is.EqualTo(roundTrip.RequestID));
            
            var outResponse = (ExceptionResponse)roundTrip.Response;
            Assert.That(inResponse.ExceptionType, Is.EqualTo(outResponse.ExceptionType));
            Assert.That(inResponse.Message, Is.EqualTo(outResponse.Message));
        }
    }
}
