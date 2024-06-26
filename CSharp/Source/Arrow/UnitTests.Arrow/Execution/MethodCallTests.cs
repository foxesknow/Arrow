﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Execution;

using NUnit.Framework;

namespace UnitTests.Arrow.Execution
{
    [TestFixture]
    public class MethodCallTests
    {
        delegate void BinaryHandler<T>(T first, T second);

        [Test]
        public void AllowFailAsync_NoMethod()
        {
            Assert.Catch(() => MethodCall.AllowFailAsync(null));
            Assert.Catch(() => MethodCall.AllowFailAsync(10, null));
        }

        [Test]
        public void AllowFailAsync_NoExceptions()
        {
            Assert.DoesNotThrow(() => MethodCall.AllowFailAsync(async () => await Task.CompletedTask));
            Assert.DoesNotThrow(() => MethodCall.AllowFailAsync(10, async state => await Task.CompletedTask));
        }

        [Test]
        public async Task AllowFailAsync_NoState_NoMethod()
        {
            var called = false;

            await MethodCall.AllowFailAsync(async () =>
            {
                await Task.Delay(100);
                called = true;
            });

            Assert.That(called, Is.True);
        }

        [Test]
        public async Task AllowFailAsync_State_NoMethod()
        {
            var flag = 0;

            await MethodCall.AllowFailAsync(42, async state =>
            {
                await Task.Delay(100);
                flag = state;
            });

            Assert.That(flag, Is.EqualTo(42));
        }

        [Test]
        public void TestAllowFail()
        {
            bool wasCalled = false;

            Action method = () =>
            {
                wasCalled = true;
                throw new Exception("should not see this");
            };

            MethodCall.AllowFail(method);
            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void TestAllowFailWithExceptionHandler()
        {
            bool methodCalled = false;
            Action method = () =>
            {
                methodCalled = true;
                throw new Exception("should not see this");
            };

            bool handlerCalled = false;
            Action<Exception> handler = (Exception e) =>
            {
                handlerCalled = true;
                Assert.That(e.Message, Is.EqualTo("should not see this"));
            };

            MethodCall.AllowFail(handler, method);
            Assert.IsTrue(methodCalled);
            Assert.IsTrue(handlerCalled);
        }

        [Test]
        public void TestRetry()
        {
            bool methodCalled = false;
            int callNumber = 0;
            Action method = () =>
            {
                callNumber++;
                if(methodCalled == false)
                {
                    methodCalled = true;
                    throw new Exception("should not see this");
                }
            };

            MethodCall.Retry(3, 0, method);
            Assert.IsTrue(methodCalled);
            Assert.That(callNumber, Is.EqualTo(2));
        }

        [Test]
        public void TestRetryWithFailure()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                Action method = () =>
                {
                    throw new ArgumentException();
                };

                MethodCall.Retry(3, 0, method);
                Assert.Fail(); // We should never get here
            });
        }

        [Test]
        public void TestInvoke1()
        {
            string result = null;
            Action<string> action = (string data) =>
            {
                result = data;
            };

            Assert.IsNull(result);
            MethodCall.Invoke(action, "hello");
            Assert.That(result, Is.EqualTo("hello"));
        }

        [Test]
        public void TestInvoke2()
        {
            int result = 0;
            BinaryHandler<int> action = (int first, int second) =>
            {
                result = first + second;
            };

            Assert.That(result, Is.EqualTo(0));
            MethodCall.Invoke(action, 4, 5);
            Assert.That(result, Is.EqualTo(9));
        }
    }
}
