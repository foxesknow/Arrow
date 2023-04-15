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
    public class RemindersTests
    {
        [Test]
        public void Initialization()
        {
            Assert.DoesNotThrow(() =>
            {
                using(var reminders = new Reminders())
                {
                }
            });
        }

        [Test]
        public void Clear()
        {
            long counter = 0;

            using(var reminders = new Reminders())
            {
                reminders.Add(DateTime.Now.AddHours(1), () => Interlocked.Increment(ref counter));
                reminders.Add(DateTime.Now.AddHours(1), () => Interlocked.Increment(ref counter));
                reminders.Add(DateTime.Now.AddHours(1), () => Interlocked.Increment(ref counter));

                Assert.That(reminders.Count, Is.EqualTo(3));

                reminders.Clear();

                Assert.That(reminders.Count, Is.EqualTo(0));
            }
        }

        [Test]
        public void Cancel()
        {
            long counter = 0;

            using(var reminders = new Reminders())
            {
                var job1 = reminders.Add(DateTime.Now.AddHours(1), () => Interlocked.Increment(ref counter));
                Assert.That(job1, Is.Not.EqualTo(ReminderID.None));

                var job2 = reminders.Add(DateTime.Now.AddHours(1), () => Interlocked.Increment(ref counter));
                Assert.That(job2, Is.Not.EqualTo(ReminderID.None));

                var job3 = reminders.Add(DateTime.Now.AddHours(1), () => Interlocked.Increment(ref counter));
                Assert.That(job3, Is.Not.EqualTo(ReminderID.None));

                Assert.That(reminders.Count, Is.EqualTo(3));

                Assert.That(reminders.Cancel(job2), Is.True);
                Assert.That(reminders.Cancel(job2), Is.False);

                Assert.That(reminders.Count, Is.EqualTo(2));
            }
        }

        [Test]
        public void CancelAndReschedule()
        {
            long counter = 0;

            using(var done = new ManualResetEvent(false))
            using(var reminders = new Reminders())
            {
                var job1 = reminders.Add(DateTime.Now.AddHours(1), () => Interlocked.Add(ref counter, 1));
                var job2 = reminders.Add(DateTime.Now.AddHours(2), () => Interlocked.Add(ref counter, 2));
                var job3 = reminders.Add(DateTime.Now.AddHours(4), () => Interlocked.Add(ref counter, 4));

                reminders.Cancel(job1);
                reminders.Cancel(job3);

                using(SwitchClock.To(new BaselineClockDriver(Clock.Now.AddHours(5))))
                {
                    reminders.Reschedule();

                    done.WaitOne(5_000);
                    Assert.That(reminders.Count, Is.EqualTo(0));
                    Assert.That(counter, Is.EqualTo(2));
                }
            }
        }

        [Test]
        public void Add_OneFired()
        {
            long counter = 0;

            using(var done = new ManualResetEvent(false))
            using(var reminders = new Reminders())
            {
                reminders.Add(DateTime.Now.AddHours(1), () => Interlocked.Add(ref counter, 1));
                reminders.Add(DateTime.Now.AddHours(1), () => Interlocked.Add(ref counter, 2));
                reminders.Add(DateTime.Now, () => 
                {
                    Interlocked.Add(ref counter, 4);
                    done.Set();
                });

                done.WaitOne(10_000);
                Assert.That(reminders.Count, Is.EqualTo(2));
                Assert.That(counter, Is.EqualTo(4));
            }
        }

        [Test]
        public void Add_WithStateData()
        {
            string name = null;

            using(var done = new ManualResetEvent(false))
            using(var reminders = new Reminders())
            {
                reminders.Add(DateTime.Now, "Jack", state => 
                {
                    name = (string)state;
                    done.Set();
                });

                done.WaitOne(10_000);
                Assert.That(reminders.Count, Is.EqualTo(0));
                Assert.That(name, Is.EqualTo("Jack"));
            }
        }

        [Test]
        public void AllFired()
        {
            long counter = 0;

            using(var done = new ManualResetEvent(false))
            using(var reminders = new Reminders())
            {
                reminders.Add(DateTime.Now.AddHours(1), () => Interlocked.Add(ref counter, 1));
                reminders.Add(DateTime.Now.AddHours(1), () => Interlocked.Add(ref counter, 2));
                reminders.Add(DateTime.Now.AddHours(1), () => Interlocked.Add(ref counter, 4));

                using(SwitchClock.To(new BaselineClockDriver(Clock.Now.AddHours(5))))
                {
                    reminders.Reschedule();

                    done.WaitOne(5_000);
                    Assert.That(reminders.Count, Is.EqualTo(0));
                    Assert.That(counter, Is.EqualTo(7));
                }
            }
        }

        [Test]
        public void RunsImmediately()
        {
            using(var runEvent = new ManualResetEvent(false))
            {
                var flag = 0;
            
                using(var reminders = new Reminders())
                {
                    reminders.Add(Clock.Now.AddMinutes(-1), () => 
                    {
                        flag = 1;
                        runEvent.Set();
                    });

                    runEvent.WaitOne(10_1000);
                    Assert.That(flag, Is.EqualTo(1));

                    Assert.That(reminders.Count, Is.EqualTo(0));
                }
            }
        }

        [Test]
        public void Reschedule()
        {
            using(var runEvent1 = new ManualResetEvent(false))
            using(var runEvent2 = new ManualResetEvent(false))
            {
                var flag1 = 0;
                var flag2 = 0;
            
                using(var reminders = new Reminders())
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

                        runEvent1.WaitOne(10_000);
                        Assert.That(flag1, Is.EqualTo(1));

                        runEvent2.WaitOne(10_000);
                        Assert.That(flag2, Is.EqualTo(1));
                    };

                    Assert.That(reminders.Count, Is.EqualTo(0));
                }
            }
        }

        [Test]
        public void CheckOrdering_1()
        {
            using(var reminders = new Reminders())
            {
                var date = DateTime.UtcNow.AddHours(5);

                var job1 = reminders.Add(date, () => {});
                var job2 = reminders.Add(date, () => {});
                var job3 = reminders.Add(date, () => {});
                var job4 = reminders.Add(date, () => {});
                var job5 = reminders.Add(date, () => {});

                var ids = reminders.GetRemindersIDs();

                Assert.That(ids[0], Is.EqualTo(job5));
                Assert.That(ids[1], Is.EqualTo(job4));
                Assert.That(ids[2], Is.EqualTo(job3));
                Assert.That(ids[3], Is.EqualTo(job2));
                Assert.That(ids[4], Is.EqualTo(job1));
            }
        }

        [Test]
        public void CheckOrdering_2()
        {
            using(var reminders = new Reminders())
            {
                var date = DateTime.UtcNow.AddHours(5);

                var job1 = reminders.Add(date.AddHours(1), () => {});
                var job2 = reminders.Add(date.AddHours(2), () => {});
                var job3 = reminders.Add(date.AddHours(1), () => {});
                var job4 = reminders.Add(date.AddHours(2), () => {});
                var job5 = reminders.Add(date.AddHours(1), () => {});

                var ids = reminders.GetRemindersIDs();

                Assert.That(ids[0], Is.EqualTo(job4));
                Assert.That(ids[1], Is.EqualTo(job2));
                Assert.That(ids[2], Is.EqualTo(job5));
                Assert.That(ids[3], Is.EqualTo(job3));
                Assert.That(ids[4], Is.EqualTo(job1));
            }
        }
    }
}
