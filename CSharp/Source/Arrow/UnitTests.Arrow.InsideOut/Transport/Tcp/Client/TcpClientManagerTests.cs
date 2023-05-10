using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.InsideOut;
using Arrow.InsideOut.Transport;
using Arrow.InsideOut.Transport.Tcp;
using Arrow.InsideOut.Transport.Tcp.Client;
using Arrow.IO;
using NUnit.Framework;

namespace UnitTests.Arrow.InsideOut.Transport.Tcp.Client
{
    [TestFixture]
    public partial class TcpClientManagerTests
    {
        private static readonly PublisherID Publisher = new(Environment.MachineName, "Jack");

        [Test]
        public void Register()
        {
            using (var manager = new TcpClientManager())
            {
                var node = manager.Register(Publisher, new("tcp://localhost:12345"));
                Assert.That(node, Is.Not.Null);
                Assert.That(manager.IsRegistered(Publisher), Is.True);
            }
        }

        [Test]
        public void RegisterTwice()
        {
            using (var manager = new TcpClientManager())
            {
                var node = manager.Register(Publisher, new("tcp://localhost:12345"));
                Assert.Catch(() => manager.Register(Publisher, new("tcp://localhost:12345")));
                Assert.Catch(() => manager.Register(Publisher, new("tcp://localhost:55555")));
            }
        }        

        [Test]
        public void TryGetNode()
        {
            using (var manager = new TcpClientManager())
            {
                manager.Register(Publisher, new("tcp://localhost:12345"));
                Assert.That(manager.TryGetNode(Publisher, out var node), Is.True);
                Assert.That(node, Is.Not.Null);
            }
        }

        [Test]
        public void TryGetNode_NotFound()
        {
            using (var manager = new TcpClientManager())
            {
                Assert.That(manager.TryGetNode(Publisher, out var node), Is.False);
                Assert.That(node, Is.Null);
            }
        }

        [Test]
        public void Disposed()
        {
            var manager = new TcpClientManager();
            manager.Dispose();

            Assert.Catch(() => manager.Register(Publisher, new("tcp://localhost:12345")));
        }

        [Test]
        [TestCase("foo://localhost:12345")]
        [TestCase("tcp://localhost:99999")]
        [TestCase("tcp://localhost")]
        public void InvalidUri(string endpoint)
        {
            using (var manager = new TcpClientManager())
            {
                Assert.Catch(() => manager.Register(Publisher, new(endpoint)));
            }
        }

        [Test]
        public async Task GetDetails()
        {
            using(var read = new MemoryStream())
            using(var write= new MemoryStream())
            using(var network = new SplitStream(read, write))
            using(var manager = new TcpClientManager(() => new NetworkClient(network)))
            {
                var requestID = new RequestID(Guid.NewGuid(), Guid.NewGuid());
                var transportResponse = new TransportResponse(NodeResponse.GetDetails, requestID, new Details()
                {
                    Commands = 
                    {
                        new("Delete")
                    },
                    Values =
                    {
                        {"Name", Value.From("Jack")},
                        {"Age", Value.From(42)},
                    }
                });

                using(var r = InsideOutEncoder.Default.EncodeToPool(transportResponse))
                {
                    await StreamSupport.Write(read, r.Buffer!, r.Start, r.Length, default);
                    read.Position = 0;
                }

                var node = manager.Register(Publisher, new("tcp://localhost:12345"));
                var details = await node.GetDetails(default);
                Assert.That(details, Is.Not.Null);

                Assert.That(details.TryGetValue<StringValue>("Name", out var name), Is.Not.Null);
                Assert.That(name.Value, Is.EqualTo("Jack"));

                Assert.That(details.TryGetValue<Int32Value>("Age", out var age), Is.Not.Null);
                Assert.That(age.Value, Is.EqualTo(42));

                Assert.That(details.Commands.Count, Is.EqualTo(1));
                Assert.That(details.Commands[0].Name, Is.EqualTo("Delete"));
            }
        }

        [Test]
        public async Task Execute()
        {
            using(var read = new MemoryStream())
            using(var write= new MemoryStream())
            using(var network = new SplitStream(read, write))
            using(var manager = new TcpClientManager(() => new NetworkClient(network)))
            {
                var requestID = new RequestID(Guid.NewGuid(), Guid.NewGuid());
                var transportResponse = new TransportResponse(NodeResponse.Execute, requestID, new ExecuteResponse()
                {
                    Success = true,
                    Message = "updated",
                    Result = "2"
                });

                using(var r = InsideOutEncoder.Default.EncodeToPool(transportResponse))
                {
                    await StreamSupport.Write(read, r.Buffer!, r.Start, r.Length, default);
                    read.Position = 0;
                }

                var node = manager.Register(Publisher, new("tcp://localhost:12345"));

                var executeRequest = new ExecuteRequest("user/update");
                var response = await node.Execute(executeRequest, default);
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Success, Is.True);
                Assert.That(response.Message, Is.EqualTo("updated"));
                Assert.That(response.Result, Is.EqualTo("2"));
            }
        }

        [Test]
        public async Task ThrowsException()
        {
            using(var read = new MemoryStream())
            using(var write= new MemoryStream())
            using(var network = new SplitStream(read, write))
            using(var manager = new TcpClientManager(() => new NetworkClient(network)))
            {
                var requestID = new RequestID(Guid.NewGuid(), Guid.NewGuid());
                var transportResponse = new TransportResponse(NodeResponse.Exception, requestID, new ExceptionResponse("invalid user")
                {
                    ExceptionType = nameof(ArgumentException)
                });

                using(var r = InsideOutEncoder.Default.EncodeToPool(transportResponse))
                {
                    await StreamSupport.Write(read, r.Buffer!, r.Start, r.Length, default);
                    read.Position = 0;
                }

                var node = manager.Register(Publisher, new("tcp://localhost:12345"));
                var executeRequest = new ExecuteRequest("user/update");

                try
                {
                    await node.Execute(executeRequest, default);
                    Assert.Fail("should have thrown an exception");
                }
                catch(ArgumentException e)
                {
                    Assert.That(e.Message, Is.EqualTo("invalid user"));
                }
                catch
                {
                    Assert.Fail("unexpected exception");
                }
            }
        }
    }
}
