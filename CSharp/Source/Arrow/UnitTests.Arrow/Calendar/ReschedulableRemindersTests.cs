using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Arrow.Calendar;
using Arrow.Calendar.ClockDrivers;
using Arrow.Calendar.Testing;

using NUnit.Framework;

namespace UnitTests.Arrow.Calendar
{
    [TestFixture]
    public class ReschedulableRemindersTests
    {
        [Test]
        public void Initialization()
        {
            Assert.DoesNotThrow(() =>
            {
                using(var reminders = new ReschedulableReminders())
                {
                }
            });
        }

        [Test]
        public void RunsImmediately()
        {
            using(var runEvent = new ManualResetEvent(false))
            {
                var flag = 0;
            
                using(var reminders = new ReschedulableReminders())
                {
                    reminders.Add(Clock.Now.AddMinutes(-1), () => 
                    {
                        flag = 1;
                        runEvent.Set();
                    });

                    runEvent.WaitOne();
                    Assert.That(flag, Is.EqualTo(1));

                    Assert.That(reminders.Count, Is.EqualTo(0));
                }
            }
        }

        [Test]
        public void RunInFuture()
        {
            using(var runEvent1 = new ManualResetEvent(false))
            using(var runEvent2 = new ManualResetEvent(false))
            {
                var flag1 = 0;
                var flag2 = 0;
            
                using(var reminders = new ReschedulableReminders())
                {
                    reminders.Add(Clock.Now.AddMinutes(10), () => 
                    {
                        flag1 = 1;
                        runEvent1.Set();
                    });

                    reminders.Add(Clock.Now.AddMinutes(7), () => 
                    {
                        flag2 = 1;
                        runEvent2.Set();
                    });

                    using(SwitchClock.To(new BaselineClockDriver(Clock.Now.AddHours(1))))
                    {
                        reminders.Reschedule();

                        runEvent1.WaitOne();
                        Assert.That(flag1, Is.EqualTo(1));

                        runEvent2.WaitOne();
                        Assert.That(flag2, Is.EqualTo(1));
                    };

                    Assert.That(reminders.Count, Is.EqualTo(0));
                }
            }
        }
    }
}
