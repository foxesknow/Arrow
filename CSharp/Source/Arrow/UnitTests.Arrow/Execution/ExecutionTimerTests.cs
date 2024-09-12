using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Execution;

using NUnit.Framework;


namespace UnitTests.Arrow.Execution
{
    [TestFixture]
    public class ExecutionTimerTests
    {
        [Test]
        public void Measure()
        {
            var called = false;
            var elapsed = ExecutionTimer.Measure(TimerMode.Nanoseconds, () => called = true);
            Assert.That(called, Is.True);
            Assert.That(elapsed, Is.Not.EqualTo(0d));
        }

        [Test]
        public void MeasureWithState()
        {
            var value = 0;
            var elapsed = ExecutionTimer.Measure(TimerMode.Nanoseconds, 99, state => value = state);
            Assert.That(value, Is.EqualTo(99));
            Assert.That(elapsed, Is.Not.EqualTo(0d));
        }
    }
}
