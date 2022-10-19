using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Threading.Tasks;

using NUnit.Framework;

namespace UnitTests.Arrow.Threading.Tasks
{
    [TestFixture]
    public class SignallerTests
    {
        [Test]
        public void Signal_NoConditions()
        {
            var signaller = new Signaller<string>();
            Assert.That(signaller.Signal("Jack"), Is.EqualTo(default(SignalResult)));
        }

        [Test]
        public void When_BadCondition()
        {
            var signaller = new Signaller<string>();
            Assert.Catch(() => signaller.When(null));
        }

        [Test]
        public void When()
        {
            var signaller = new Signaller<string>();
            var task = signaller.When(static s => s.Length ==4);
            Assert.That(task, Is.Not.Null);
        }

        [Test]
        public void ImplementationWhen()
        {
            var signaller = new Signaller<string>();
            var token = signaller.ImplementationWhen(static s => s.Length ==4);
            Assert.That(token.Task, Is.Not.Null);
            Assert.That(token.ID, Is.Not.EqualTo(0));
        }

        [Test]
        public async Task When_OneCondition()
        {
            var signaller = new Signaller<string>();
            var task = signaller.When(static s => s.Length == 4);
            
            var inserterTask = Task.Run(async () =>
            {
                await Task.Delay(1000);
                return signaller.Signal("Jack");
            });

            var name = await task;
            Assert.That(name, Is.EqualTo("Jack"));

            var signalResult = await inserterTask;
            Assert.That(signalResult.Successful, Is.EqualTo(1));
            Assert.That(signalResult.Failed, Is.EqualTo(0));
            Assert.That(signalResult.Threw, Is.EqualTo(0));
        }

        [Test]
        public async Task When_TwoCondition_BothMatch()
        {
            var signaller = new Signaller<string>();
            var task1 = signaller.When(static s => s.Length == 4);
            var task2 = signaller.When(static s => s == "Jack");
            
            var inserterTask = Task.Run(async () =>
            {
                await Task.Delay(1000);
                return signaller.Signal("Jack");
            });

            Assert.That(await task1, Is.EqualTo("Jack"));
            Assert.That(await task2, Is.EqualTo("Jack"));

            var signalResult = await inserterTask;
            Assert.That(signalResult.Successful, Is.EqualTo(2));
            Assert.That(signalResult.Failed, Is.EqualTo(0));
            Assert.That(signalResult.Threw, Is.EqualTo(0));
        }

        [Test]
        public async Task When_TwoCondition_OneMatches()
        {
            var signaller = new Signaller<string>();
            var task1 = signaller.When(static s => s == "Ben");
            var task2 = signaller.When(static s => s == "Jack");
            
            var inserterTask = Task.Run(async () =>
            {
                await Task.Delay(1000);
                return signaller.Signal("Ben");
            });

            Assert.That(await task1, Is.EqualTo("Ben"));
            Assert.That(task2.IsCompleted, Is.False);

            var signalResult = await inserterTask;
            Assert.That(signalResult.Successful, Is.EqualTo(1));
            Assert.That(signalResult.Failed, Is.EqualTo(0));
            Assert.That(signalResult.Threw, Is.EqualTo(0));
        }

        [Test]
        public async Task When_ConditionThrowsException()
        {
            var signaller = new Signaller<string>();
            var task = signaller.When(static s => throw new InvalidOperationException());
            
            var inserterTask = Task.Run(async () =>
            {
                await Task.Delay(1000);
                return signaller.Signal("Ben");
            });

            Assert.ThrowsAsync<InvalidOperationException>(async () => await task);

            var signalResult = await inserterTask;
            Assert.That(signalResult.Successful, Is.EqualTo(0));
            Assert.That(signalResult.Failed, Is.EqualTo(0));
            Assert.That(signalResult.Threw, Is.EqualTo(1));
        }

        [Test]
        public async Task When_TwoCondition_OneThrows()
        {
            var signaller = new Signaller<string>();
            var task1 = signaller.When(static s => s == "Ben");
            var task2 = signaller.When(static s => throw new InvalidOperationException());
            
            var inserterTask = Task.Run(async () =>
            {
                await Task.Delay(1000);
                return signaller.Signal("Ben");
            });

            Assert.That(await task1, Is.EqualTo("Ben"));
            Assert.ThrowsAsync<InvalidOperationException>(async () => await task2);

            var signalResult = await inserterTask;
            Assert.That(signalResult.Successful, Is.EqualTo(1));
            Assert.That(signalResult.Failed, Is.EqualTo(0));
            Assert.That(signalResult.Threw, Is.EqualTo(1));
        }

        [Test]
        public async Task Cancel()
        {
            var signaller = new Signaller<string>();
            var task = signaller.When(static s => throw new InvalidOperationException());
            
            var inserterTask = Task.Run(async () =>
            {
                await Task.Delay(1000);
                return signaller.Cancel(task);
            });

            Assert.CatchAsync<OperationCanceledException>(async () => await task);
            Assert.That(await inserterTask, Is.True);
        }

        [Test]
        public void Cancel_DefaultWhenToken()
        {
            var signaller = new Signaller<string>();
            var token = default(Signaller<string>.WhenToken);

            Assert.That(signaller.Cancel(token), Is.False);
        }

        [Test]
        public void Cancel_WhenToken()
        {
            var signaller = new Signaller<string>();
            var token = signaller.ImplementationWhen(static s => s == "Jack");

            Assert.That(signaller.Cancel(token), Is.True);
        }

        [Test]
        public void Remove()
        {
            var signaller = new Signaller<string>();
            var task = signaller.When(static s => s == "Jack");

            Assert.That(signaller.Remove(task), Is.True);
            Assert.That(signaller.Remove(task), Is.False);
        }

        [Test]
        public void Remove_DefaultWhenToken()
        {
            var signaller = new Signaller<string>();
            var token = default(Signaller<string>.WhenToken);

            Assert.That(signaller.Remove(token), Is.False);
        }

        [Test]
        public void Remve_WhenToken()
        {
            var signaller = new Signaller<string>();
            var token = signaller.ImplementationWhen(static s => s == "Jack");

            Assert.That(signaller.Remove(token), Is.True);
            Assert.That(signaller.Remove(token), Is.False);
        }

        [Test]
        public async Task ExecuteAndWait_Action_OneCondition()
        {
            var signaller = new Signaller<string>();

            var flag = false;
            var task = signaller.ExecuteAndWait
            (
                static s => s.Length == 4,
                () => flag = true
            );

            var inserterTask = Task.Run(async () =>
            {
                await Task.Delay(1000);
                return signaller.Signal("Jack");
            });

            var name = await task;
            Assert.That(name, Is.EqualTo("Jack"));
            Assert.That(flag, Is.True);

            var signalResult = await inserterTask;
            Assert.That(signalResult.Successful, Is.EqualTo(1));
            Assert.That(signalResult.Failed, Is.EqualTo(0));
            Assert.That(signalResult.Threw, Is.EqualTo(0));
        }

        [Test]
        public async Task ExecuteAndWait_Function_OneCondition()
        {
            var signaller = new Signaller<string>();

            var flag = false;
            var task = signaller.ExecuteAndWait
            (
                static s => s.Length == 4,
                async () => 
                {
                    await Task.Delay(500);
                    flag = true;
                }
            );

            var inserterTask = Task.Run(async () =>
            {
                await Task.Delay(1000);
                return signaller.Signal("Jack");
            });

            var name = await task;
            Assert.That(name, Is.EqualTo("Jack"));
            Assert.That(flag, Is.True);

            var signalResult = await inserterTask;
            Assert.That(signalResult.Successful, Is.EqualTo(1));
            Assert.That(signalResult.Failed, Is.EqualTo(0));
            Assert.That(signalResult.Threw, Is.EqualTo(0));
        }

        [Test]
        public void ExecuteAndWait_FunctionThrowsException()
        {
            var signaller = new Signaller<string>();
            var task = signaller.ExecuteAndWait
            (
                static s => throw new ArgumentOutOfRangeException(),
                async () =>
                {
                    await Task.Delay(500);
                    throw new InvalidOperationException();
                }
            );

            Assert.ThrowsAsync<InvalidOperationException>(async () => await task);
        }

        [Test]
        public async Task ExecuteAndWait_ConditionThrowsException()
        {
            var signaller = new Signaller<string>();
            var flag = false;

            var task = signaller.ExecuteAndWait
            (
                static s => throw new ArgumentOutOfRangeException(),
                async () =>
                {
                    await Task.Delay(500);
                    flag = true;
                }
            );

            var inserterTask = Task.Run(async () =>
            {
                await Task.Delay(1000);
                return signaller.Signal("Jack");
            });

            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await task);
            Assert.That(flag, Is.True);

            var signalResult = await inserterTask;
            Assert.That(signalResult.Successful, Is.EqualTo(0));
            Assert.That(signalResult.Failed, Is.EqualTo(0));
            Assert.That(signalResult.Threw, Is.EqualTo(1));
        }
    }
}
