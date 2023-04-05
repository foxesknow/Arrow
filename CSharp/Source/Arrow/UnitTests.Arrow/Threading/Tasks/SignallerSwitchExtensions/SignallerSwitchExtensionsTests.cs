using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Arrow;
using Arrow.Threading.Tasks;
using Arrow.Threading.Tasks.SignallerSwitch;

using NUnit.Framework;

namespace UnitTests.Arrow.Threading.Tasks.SignallerSwitchExtensions
{
    [TestFixture]
    public class SignallerSwitchExtensionsTests
    {
        [Test]
        [TestCase("Ben", 1)]
        [TestCase("Kate", 2)]
        [TestCase("Jack", 3)]
        [TestCase("Sawyer", 4)]
        public async Task Switch(string name, int expectedValue)
        {
            var signaller = new Signaller<string>();

            string functionValue = null;

            var task = signaller.Switch(_ => functionValue = "Bob")
                                .Case(c => c.Value == "Ben", c => 1)
                                .Case(c => c.Value == "Kate", c => 2)
                                .Case(c => c.Value == "Jack", c => 3)
                                .Case(c => c.Value == "Sawyer", c => 4)
                                .Execute();

            // Now signal some data
            signaller.Signal(name);
            var (result, data) = await task;

            Assert.That(functionValue, Is.EqualTo("Bob"));
            Assert.That(result, Is.EqualTo(expectedValue));
            Assert.That(data, Is.EqualTo(name));
        }

        [Test]
        [TestCase("Ben", 1)]
        [TestCase("Kate", 2)]
        [TestCase("Jack", 3)]
        [TestCase("Sawyer", 4)]
        public async Task Switch_MatchOnSecondSignal(string name, int expectedValue)
        {
            var signaller = new Signaller<string>();

            string functionValue = null;

            var task = signaller.Switch(_ => functionValue = "Bob")
                                .Case(c => c.Value == "Ben", c => 1)
                                .Case(c => c.Value == "Kate", c => 2)
                                .Case(c => c.Value == "Jack", c => 3)
                                .Case(c => c.Value == "Sawyer", c => 4)
                                .Execute();

            // Now signal some data
            signaller.Signal("Foo");
            signaller.Signal(name);
            var (result, data) = await task;

            Assert.That(functionValue, Is.EqualTo("Bob"));
            Assert.That(result, Is.EqualTo(expectedValue));
            Assert.That(data, Is.EqualTo(name));
        }

        [Test]
        [TestCase("Ben", 1)]
        [TestCase("Kate", 2)]
        [TestCase("Jack", 3)]
        [TestCase("Sawyer", 4)]
        public async Task Switch_NoResult(string name, int expectedValue)
        {
            var signaller = new Signaller<string>();

            string functionValue = null;

            var task = signaller.Switch(_ => functionValue = "Bob")
                                .Case(c => c.Value == "Ben", c => {})
                                .Case(c => c.Value == "Kate", c => {})
                                .Case(c => c.Value == "Jack", c => {})
                                .Case(c => c.Value == "Sawyer", c => {})
                                .Execute();

            // Now signal some data
            signaller.Signal(name);
            var (result, data) = await task;

            Assert.That(functionValue, Is.EqualTo("Bob"));
            Assert.That(result, Is.EqualTo(Unit.Default));
            Assert.That(data, Is.EqualTo(name));
        }

        [Test]
        [TestCase("Ben", 1)]
        [TestCase("Kate", 2)]
        [TestCase("Jack", 3)]
        [TestCase("Sawyer", 4)]
        public async Task Switch_FullAsync(string name, int expectedValue)
        {
            var signaller = new Signaller<string>();

            string functionValue = null;

            var task = signaller.Switch(async _ => functionValue = await DelayAndReturn("Bob"))
                                .Case(c => c.Value == "Ben", c => DelayAndReturn(1))
                                .Case(c => c.Value == "Kate", c => DelayAndReturn(2))
                                .Case(c => c.Value == "Jack", c => DelayAndReturn(3))
                                .Case(c => c.Value == "Sawyer", c => DelayAndReturn(4))
                                .Execute();

            // Now signal some data
            signaller.Signal(name);
            var (result, data) = await task;

            Assert.That(functionValue, Is.EqualTo("Bob"));
            Assert.That(result, Is.EqualTo(expectedValue));
            Assert.That(data, Is.EqualTo(name));
        }

        [Test]
        [TestCase("Ben", 1)]
        [TestCase("Kate", 2)]
        [TestCase("Jack", 3)]
        [TestCase("Sawyer", 4)]
        public async Task Switch_Timeout_Match(string name, int expectedValue)
        {
            var signaller = new Signaller<string>();

            string functionValue = null;

            var task = signaller.Switch(_ => functionValue = "Bob")
                                .Case(c => c.Value == "Ben", c => 1)
                                .Case(c => c.Value == "Kate", c => 2)
                                .Case(c => c.Value == "Jack", c => 3)
                                .Case(c => c.Value == "Sawyer", c => 4)
                                .TimeoutAfter(TimeSpan.FromSeconds(60))
                                .Execute();

            // Now signal some data
            signaller.Signal(name);
            var (result, data) = await task;

            Assert.That(functionValue, Is.EqualTo("Bob"));
            Assert.That(result, Is.EqualTo(expectedValue));
            Assert.That(data, Is.EqualTo(name));
        }

        [Test]
        [TestCase("Hurley", 1)]
        public void Switch_Timeout_NoMatch(string name, int expectedValue)
        {
            var signaller = new Signaller<string>();

            var task = signaller.Switch(_ => Task.CompletedTask)
                                .Case(c => c.Value == "Ben", c => 1)
                                .Case(c => c.Value == "Kate", c => 2)
                                .Case(c => c.Value == "Jack", c => 3)
                                .Case(c => c.Value == "Sawyer", c => 4)
                                .TimeoutAfter(TimeSpan.FromSeconds(1))
                                .Execute();

            // Now signal some data
            signaller.Signal(name);
            Assert.CatchAsync(async () => await task);
        }

        [Test]
        [TestCase("Ben", 1)]
        [TestCase("Kate", 2)]
        [TestCase("Jack", 3)]
        [TestCase("Sawyer", 4)]
        public void Switch_Cancel(string name, int expectedValue)
        {
            using(var cts = new CancellationTokenSource())
            {
                var signaller = new Signaller<string>();

                string functionValue = null;

                var task = signaller.Switch(_ => functionValue = "Bob")
                                    .Case(c => c.Value == "Ben", c => 1)
                                    .Case(c => c.Value == "Kate", c => 2)
                                    .Case(c => c.Value == "Jack", c => 3)
                                    .Case(c => c.Value == "Sawyer", c => 4)
                                    .CancelWhen(cts.Token)
                                    .Execute();

                // Now signal some data
                cts.Cancel();
                signaller.Signal(name);
                Assert.CatchAsync(async () => await task);

                /*
                 * functionValue will be set because the the function passed to Switch() will
                 * run when Execute() is called, which is before we trigger the cancel
                 */
                Assert.That(functionValue, Is.EqualTo("Bob"));
            }
        }

        [Test]
        [TestCase("Ben", 1)]
        [TestCase("Kate", 2)]
        [TestCase("Jack", 3)]
        [TestCase("Sawyer", 4)]
        public void Switch_Cancel_BeforeSwitch(string name, int expectedValue)
        {
            using(var cts = new CancellationTokenSource())
            {
                cts.Cancel();
                var signaller = new Signaller<string>();

                string functionValue = null;

                var task = signaller.Switch(_ => functionValue = "Bob")
                                    .Case(c => c.Value == "Ben", c => 1)
                                    .Case(c => c.Value == "Kate", c => 2)
                                    .Case(c => c.Value == "Jack", c => 3)
                                    .Case(c => c.Value == "Sawyer", c => 4)
                                    .CancelWhen(cts.Token)
                                    .Execute();

                // Now signal some data                
                signaller.Signal(name);
                Assert.CatchAsync(async () => await task);
                Assert.That(functionValue, Is.Null);
            }
        }

        static async Task<T> DelayAndReturn<T>(T value)
        {
            await Task.Delay(1000);
            return value;
        }

        static Task<T> Return<T>(T value)
        {
            return Task.FromResult(value);
        }
    }
}
