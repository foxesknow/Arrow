using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

using Arrow.InsideOut.Plugins;
using Arrow.InsideOut.Transport;
using Arrow.InsideOut.Transport.Messaging.Client;
using Arrow.InsideOut.Transport.Messaging.Server;
using Arrow.Xml.ObjectCreation;
using Arrow.Execution;

using NUnit.Framework;
using Arrow.InsideOut;
using Arrow.Threading.Tasks;

namespace UnitTests.Arrow.InsideOut.Transport.Messaging.Client
{
    [TestFixture]
    public class MessagingClientManagerTests
    {
        private static readonly PublisherID Publisher = new(Environment.MachineName, "Jack");

        [Test]
        public void Register()
        {
            using(var manager = new MessagingClientManager())
            {
                var node = manager.Register(Publisher, new("memtopic://inmemory/InsideOut"));
                Assert.That(node, Is.Not.Null);
            }
        }

        [Test]
        public void RegisterTwice()
        {
            using(var manager = new MessagingClientManager())
            {
                manager.Register(Publisher, new("memtopic://inmemory/InsideOut"));
                Assert.Catch(() => manager.Register(Publisher, new("memtopic://inmemory/InsideOut")));
            }
        }

        [Test]
        public void TryGetNode()
        {
            using(var manager = new MessagingClientManager())
            {
                Assert.That(manager.TryGetNode(Publisher, out var _), Is.False);

                manager.Register(Publisher, new("memtopic://inmemory/InsideOut"));
                Assert.That(manager.TryGetNode(Publisher, out var node), Is.True);
                Assert.That(node, Is.Not.Null);
            }
        }

        [Test]
        public void CancelOutstandingTasks()
        {
            using(var manager = new MessagingClientManager())
            {
                var node = manager.Register(Publisher, new("memtopic://inmemory/InsideOut"));
                var task = node.GetDetails();

                manager.CancelOutstandingTasks();
                Assert.CatchAsync(async () => await task);
            }
        }

        [Test]
        public void CancelFromToken()
        {
            using(var cts = new CancellationTokenSource())
            using(var manager = new MessagingClientManager())
            {
                var node = manager.Register(Publisher, new("memtopic://inmemory/InsideOut"));
                var task = node.GetDetails(cts.Token);

                cts.Cancel();
                Assert.CatchAsync(async () => await task);
            }
        }

        [Test]
        public async Task GetDetails()
        {
            await using(var listener = await MakeListener())
            using(var manager = new MessagingClientManager())
            {
                var node = manager.Register(Publisher, new("memtopic://inmemory/InsideOut"));
                var details = await node.GetDetails();
                Assert.That(details, Is.Not.Null);                
            }
        }

        [Test]
        public async Task Execute()
        {
            await using(var listener = await MakeListener())
            using(var manager = new MessagingClientManager())
            {
                var node = manager.Register(Publisher, new("memtopic://inmemory/InsideOut"));

                var executeRequest = new ExecuteRequest("Calculator/Divide")
                {
                    Arguments =
                    {
                        new DecimalArgument("lhs"){Value = 100},
                        new DecimalArgument("rhs"){Value = 5},
                    }
                };

                var response = await node.Execute(executeRequest);
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Result, Is.EqualTo(Value.From(20m)));
            }
        }

        [Test]
        public async Task ThrowsException()
        {
            await using(var listener = await MakeListener())
            using(var manager = new MessagingClientManager())
            {
                var node = manager.Register(Publisher, new("memtopic://inmemory/InsideOut"));

                var executeRequest = new ExecuteRequest("Calculator/Divide")
                {
                    Arguments =
                    {
                        new DecimalArgument("lhs"){Value = 100},
                        new DecimalArgument("rhs"){Value = 0},  // <-- divide by zero!
                    }
                };

                Assert.CatchAsync(async () => await node.Execute(executeRequest));
            }
        }

        private async ValueTask<IAsyncDisposable> MakeListener()
        {
            var document = new XmlDocument();
            document.LoadXml(s_PluginXml);

            var plugin = XmlCreation.Create<InsideOutPlugin>(document.DocumentElement);

            var listener = new ListenerPlugin(plugin)
            {
                Endpoint = new("memtopic://inmemory/InsideOut"),
                InstanceName = "Jack"
            };

            ((ISupportInitialize)listener).EndInit();
            await listener.TestStart().ContinueOnAnyContext();

            return AsyncDisposer.Make(async () =>
            {
                await listener.TestStop().ContinueOnAnyContext();
                listener.Dispose();
            });
        }

        private static string s_PluginXml = @"
            <Plugin type=""Arrow.InsideOut.Plugins.InsideOutPlugin, Arrow.InsideOut"">
				<Node name=""Calculator"" type=""Arrow.InsideOut.Nodes.CalculatorNode, Arrow.InsideOut"" />					
			</Plugin>
        ";
    }
}
